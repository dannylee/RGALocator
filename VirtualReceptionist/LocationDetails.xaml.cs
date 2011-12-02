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
using Microsoft.Phone.Shell;

using System.IO;
using System.Xml;
using System.Xml.Linq;
using VirtualReceptionist.BusinessObjects;

namespace VirtualReceptionist
{
    public partial class LocationDetails : PhoneApplicationPage
    {
        private WebClient locationClient;

        private string locationId;

        private string locationName = "";

        Setting<string> savedUserName = new Setting<string>("UserName", "");

        public LocationDetails()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;

            var backButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar.back.rest.png", UriKind.Relative),
                Text = "Back"
            };
            backButton.Click += btnBack_OnClick;
            ApplicationBar.Buttons.Add(backButton);

            var settingsButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar.feature.settings.rest.png", UriKind.Relative),
                Text = "Settings"
            };
            settingsButton.Click += btnSettings_OnClick;
            ApplicationBar.Buttons.Add(settingsButton);

            if (!string.IsNullOrEmpty(this.savedUserName.Value))
            {
                var checkInButton = new ApplicationBarIconButton
                {
                    IconUri = new Uri("/Images/appbar.upload.rest.png", UriKind.Relative),
                    Text = "Check In"
                };
                checkInButton.Click += btnCheckIn_OnClick;
                ApplicationBar.Buttons.Add(checkInButton);

            }

            this.Loaded += LocationDetails_Loaded;
        }

        private void LocationDetails_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.NavigationContext.QueryString.ContainsKey("locationId") ||
                string.IsNullOrEmpty(this.NavigationContext.QueryString["locationId"]))
            {
                return;
            }
            locationId = this.NavigationContext.QueryString["locationId"];
            locationClient = new WebClient();
            locationClient.OpenReadCompleted += OnEmployeeServiceOpenReadComplete;
            locationClient.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/employees/?location=" + locationId));
        }

        private void loadEmployees()
        {
            
        }

        private void OnEmployeeServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            var reader = new StreamReader(e.Result);
            var result = reader.ReadToEnd();
            XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
            XDocument xml = XDocument.Load(XMLReader);

            var xColleagues = from emp in xml.Descendants("employee")
                              select emp;
            string colleagueStr = string.Empty;
            this.lstColleagues.Items.Clear();
            foreach (var xColleague in xColleagues)
            {
                var colleague = new Colleague
                {
                    ColleagueID = xColleague.Element("guid").Value,
                    FirstName = xColleague.Element("firstname").Value,
                    LastName = xColleague.Element("lastname").Value,
                    Title = xColleague.Element("title").Value
                };
                this.lstColleagues.Items.Add(colleague);
                locationName = xColleague.Element("currentlocation").Value;
            }
            this.txtIntro.Text = string.Format("Colleagues at {0}:",locationName);

        }

        private void lstColleagues_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstColleagues.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/ColleagueDetails.xaml?colleagueId=" + this.lstColleagues.SelectedValue + "&locationId=" + locationId, UriKind.Relative));
            }
        }
        public void btnBack_OnClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml?activeTab=locations", UriKind.Relative));
        }


        public void btnSettings_OnClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        public void btnCheckIn_OnClick(object sender, EventArgs e)
        {
            locationClient = new WebClient();
            locationClient.OpenReadCompleted += OnCheckInServiceOpenReadComplete;
            locationClient.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/employees/" + this.savedUserName.Value + "?location=" + locationId));
        }

        private void OnCheckInServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            var reader = new StreamReader(e.Result);
            var result = reader.ReadToEnd();
            XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
            XDocument xml = XDocument.Load(XMLReader);

            var success = Boolean.Parse(xml.Root.Value);
            if (success)
            {
                MessageBox.Show("You have checked in to " + locationName);
            }

        }

    }
}