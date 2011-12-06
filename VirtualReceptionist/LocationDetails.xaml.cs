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

        private string outOfOfficeLocationId = "bca88939-68dc-467d-a354-d733c9a5cce2";

        Setting<string> savedUserName = new Setting<string>("UserName", "");

        Setting<Boolean> savedClockedIn = new Setting<Boolean>("ClockedIn", false);

        Setting<string> savedUserLocationId = new Setting<string>("UserLocationId", "");

        public LocationDetails()
        {
            InitializeComponent();

            

            this.Loaded += LocationDetails_Loaded;
        }

        private void setUpApplicationBar()
        {
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

                if (locationId != outOfOfficeLocationId)
                {
                    if (this.savedUserLocationId.Value == locationId)
                    {
                        var checkOutButton = new ApplicationBarIconButton
                        {
                            IconUri = new Uri("/Images/appbar.checkout.rest.png", UriKind.Relative),
                            Text = "Check Out"
                        };
                        checkOutButton.Click += btnCheckOut_OnClick;
                        ApplicationBar.Buttons.Add(checkOutButton);
                    }
                    else
                    {
                        var checkInButton = new ApplicationBarIconButton
                        {
                            IconUri = new Uri("/Images/appbar.checkin.rest.png", UriKind.Relative),
                            Text = "Check In"
                        };
                        checkInButton.Click += btnCheckIn_OnClick;
                        ApplicationBar.Buttons.Add(checkInButton);
                    }
                }
                    
                 

            }

                
        }

        private void LocationDetails_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.NavigationContext.QueryString.ContainsKey("locationId") ||
                string.IsNullOrEmpty(this.NavigationContext.QueryString["locationId"]))
            {
                return;
            }
            locationId = this.NavigationContext.QueryString["locationId"];
            this.setUpApplicationBar();
            loadEmployees();
            
        }

        private void loadEmployees()
        {
            locationClient = new WebClient();
            locationClient.OpenReadCompleted += OnEmployeeServiceOpenReadComplete;
            locationClient.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/employees/?location=" + locationId + "&date=" + DateTime.Now.ToString()));
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
                this.NavigationService.Navigate(new Uri("/ColleagueDetails.xaml?colleagueId=" + this.lstColleagues.SelectedValue + "&locationId=" + locationId + "&date=" + DateTime.Now.ToString(), UriKind.Relative));
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
            var url = "http://locator.rgahosting.com/LocatorService.svc/employees/" + this.savedUserName.Value + "?location=" + locationId + "&date=" + DateTime.Now.ToString();
            locationClient.OpenReadAsync(new Uri(url));
        }

        private void OnCheckInServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("There was a problem checking you in - please check your username in settings");
            }
            else
            {
                var reader = new StreamReader(e.Result);
                var result = reader.ReadToEnd();
                XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
                XDocument xml = XDocument.Load(XMLReader);

                var success = Boolean.Parse(xml.Root.Value);
                if (success)
                {
                    this.savedUserLocationId.Value = locationId;
                    loadEmployees();
                    this.setUpApplicationBar();
                    MessageBox.Show("You have checked in to " + locationName);
                }
            }
                

        }

        public void btnCheckOut_OnClick(object sender, EventArgs e)
        {
            locationClient = new WebClient();
            locationClient.OpenReadCompleted += OnCheckOutServiceOpenReadComplete;
            var url = "http://locator.rgahosting.com/LocatorService.svc/employees/" + this.savedUserName.Value + "?location=" + outOfOfficeLocationId + "&date=" + DateTime.Now.ToString();
            locationClient.OpenReadAsync(new Uri(url));
        }

        private void OnCheckOutServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("There was a problem checking you out - please check your username in settings");
            }
            else
            {
                var reader = new StreamReader(e.Result);
                var result = reader.ReadToEnd();
                XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
                XDocument xml = XDocument.Load(XMLReader);

                var success = Boolean.Parse(xml.Root.Value);
                if (success)
                {
                    this.savedUserLocationId.Value = outOfOfficeLocationId;
                    loadEmployees();
                    this.setUpApplicationBar();
                    MessageBox.Show("You have been checked out.");
                }
            }


        }

    }
}