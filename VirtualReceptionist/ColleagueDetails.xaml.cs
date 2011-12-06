using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using VirtualReceptionist.BusinessObjects;

namespace VirtualReceptionist
{
    public partial class ColleagueDetails : PhoneApplicationPage
    {
        private WebClient client;

        private string userName;

        private Colleague colleague;

        public string locationId = "";

        Setting<string> savedUserDisplayName = new Setting<string>("UserDisplayName", "");

        Setting<string> savedUserLocation = new Setting<string>("UserLocation", "");


        public ColleagueDetails()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!this.NavigationContext.QueryString.ContainsKey("userName") ||
                string.IsNullOrEmpty(this.NavigationContext.QueryString["userName"]))
            {
                return;
            }
            userName = this.NavigationContext.QueryString["userName"];

            if (this.NavigationContext.QueryString.ContainsKey("locationId") &&
                !string.IsNullOrEmpty(this.NavigationContext.QueryString["locationId"]))
            {
                locationId = this.NavigationContext.QueryString["locationId"];
            }
            

            client = new WebClient();
            client.OpenReadCompleted += OnEmployeeServiceOpenReadComplete;
            client.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/employees/" + userName + "?date=" + DateTime.Now.ToString()));
        }

        private void OnEmployeeServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            var reader = new StreamReader(e.Result);
            var result = reader.ReadToEnd();
            XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
            XDocument data = XDocument.Load(XMLReader);
            
            var query = from emp in data.Elements("employee")
                        select new Colleague
                        {
                            ColleagueID = emp.Element("guid").Value,
                            FirstName = emp.Element("firstname").Value,
                            LastName = emp.Element("lastname").Value,
                            Title = emp.Element("title").Value,
                            CurrentLocation = emp.Element("currentlocation").Value,
                            PhotoURL = string.Format("http://{0}",emp.Element("photourl").Value),
                            ThumbnailURL = string.Format("http://{0}",emp.Element("thumbnailurl").Value),
                            MobilePhone = emp.Element("mobilephonenumber").Value
                        };
            colleague = query.SingleOrDefault();
            this.txtIntro.Visibility = Visibility.Collapsed;
            this.DataContext = colleague;
            this.ThumbnailStoryboard.Begin();

        }

        public void btnBack_OnClick(object sender, EventArgs e)
        {
            var backURI = (string.IsNullOrEmpty(locationId)) ? "/MainPage.xaml" : "/LocationDetails.xaml?locationId=" + locationId;
            this.NavigationService.Navigate(new Uri(backURI, UriKind.Relative));
        }

        public void btnSettings_OnClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
        /*
        public void btnCheckIn_OnClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml?activeTab=locations", UriKind.Relative));
        }
         */
        public void txtMobilePhone_MouseLeftButtonUp(object sender, EventArgs e)
        {
            PhoneCallTask phoneLauncher = new PhoneCallTask();
            phoneLauncher.DisplayName = string.Format("{0} {1}",txtFirstName.Text,txtLastName.Text);
            phoneLauncher.PhoneNumber = txtMobilePhone.Text;
            if (phoneLauncher.PhoneNumber.Length > 0)
            {
                phoneLauncher.Show();
            }
            
        }

        public void txtPage_MouseLeftButtonUp(object sender, EventArgs e)
        {
            SmsComposeTask smsLaucher = new SmsComposeTask();
            smsLaucher.To = txtMobilePhone.Text;
            smsLaucher.Body = string.Format("Hi, {0}, I am waiting for you at {1} - please meet me here - thanks, {2}.", this.txtFirstName.Text, this.savedUserLocation.Value, this.savedUserDisplayName.Value);
            if (smsLaucher.To.Length > 0)
            {
                smsLaucher.Show();
            }

        }
    }
}