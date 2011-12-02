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


using System.IO;
using System.Xml;
using System.Xml.Linq;
using VirtualReceptionist.BusinessObjects;

namespace VirtualReceptionist
{
    public partial class ColleagueDetails : PhoneApplicationPage
    {
        private WebClient client;

        private string colleagueId;

        private Colleague colleague;

        public string locationId = "";


        public ColleagueDetails()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!this.NavigationContext.QueryString.ContainsKey("colleagueId") ||
                string.IsNullOrEmpty(this.NavigationContext.QueryString["colleagueId"]))
            {
                return;
            }
            colleagueId = this.NavigationContext.QueryString["colleagueId"];

            if (this.NavigationContext.QueryString.ContainsKey("locationId") &&
                !string.IsNullOrEmpty(this.NavigationContext.QueryString["locationId"]))
            {
                locationId = this.NavigationContext.QueryString["locationId"];
            }
            

            client = new WebClient();
            client.OpenReadCompleted += OnEmployeeServiceOpenReadComplete;
            client.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/employees/?date=" + DateTime.Now.ToString()));
        }

        private void OnEmployeeServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            var reader = new StreamReader(e.Result);
            var result = reader.ReadToEnd();
            XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
            XDocument data = XDocument.Load(XMLReader);
            
            var query = from emp in data.Elements("ArrayOfemployee").Elements("employee")
                        where emp.Element("guid").Value == colleagueId
                        select new Colleague
                        {
                            ColleagueID = emp.Element("guid").Value,
                            FirstName = emp.Element("firstname").Value,
                            LastName = emp.Element("lastname").Value,
                            Title = emp.Element("title").Value,
                            CurrentLocation = emp.Element("currentlocation").Value,
                            PhotoURL = string.Format("http://{0}",emp.Element("photoUrl").Value),
                            ThumbnailURL = string.Format("http://{0}",emp.Element("thumbnailUrl").Value)
                        };
            colleague = query.SingleOrDefault();
            this.txtIntro.Visibility = Visibility.Collapsed;
            this.DataContext = colleague;

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
    }
}