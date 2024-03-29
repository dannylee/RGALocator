﻿using System;
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
    public partial class MainPage : PhoneApplicationPage
    {
        private WebClient employeeClient;

        private WebClient locationClient;

        Setting<string> savedUserName = new Setting<string>("UserName", "");

        Setting<string> savedUserDisplayName = new Setting<string>("UserDisplayName", "");

        Setting<string> savedUserLocation = new Setting<string>("UserLocation", "");

        Setting<string> savedUserLocationId = new Setting<string>("UserLocationId", "");

        private Setting<Boolean> savedClockedIn = new Setting<Boolean>("ClockedIn", false);

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;

            this.setUpApplicationBar();
        }

        private void setUpApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            var settingsButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar.feature.settings.rest.png", UriKind.Relative),
                Text = "Settings"
            };
            settingsButton.Click += btnSettings_OnClick;
            ApplicationBar.Buttons.Add(settingsButton);

            
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            

            employeeClient = new WebClient();
            employeeClient.OpenReadCompleted += OnEmployeeServiceOpenReadComplete;
            employeeClient.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/employees/?date=" + DateTime.Now.ToString()));
            
            locationClient = new WebClient();
            locationClient.OpenReadCompleted += OnLocationServiceOpenReadComplete;
            locationClient.OpenReadAsync(new Uri("http://locator.rgahosting.com/LocatorService.svc/locations/"));
            
        }

        protected override void  OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
 	        base.OnNavigatedTo(e);

            
            if (!this.NavigationContext.QueryString.ContainsKey("activeTab") ||
                string.IsNullOrEmpty(this.NavigationContext.QueryString["activeTab"]))
            {
                return;
            }
            var activeTab = this.NavigationContext.QueryString["activeTab"];
            if (activeTab == "locations")
            {
                this.Pivot.SelectedItem = pvtLocations;
            }
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
            foreach (var xColleague in xColleagues)
            {
                var colleague = new Colleague
                {
                    ColleagueID = xColleague.Element("guid").Value,
                    FirstName = xColleague.Element("firstname").Value,
                    LastName = xColleague.Element("lastname").Value,
                    Title = xColleague.Element("title").Value,
                    UserName = xColleague.Element("username").Value,
                    CurrentLocation = xColleague.Element("currentlocation").Value,
                    CurrentLocationId = xColleague.Element("currentlocationguid").Value
                };
                this.lstColleagues.Items.Add(colleague);

                if (!string.IsNullOrEmpty(this.savedUserName.Value) && this.savedUserName.Value.ToLower() == xColleague.Element("username").Value)
                {
                    var clockedIn = (xColleague.Element("currentlocation").Value != "Out of the office");
                    this.savedClockedIn.Value = clockedIn;
                    /*
                    if (clockedIn)
                    {
                        var checkOutButton = new ApplicationBarIconButton
                        {
                            IconUri = new Uri("/Images/appbar.checkout.rest.png", UriKind.Relative),
                            Text = "Check Out"
                        };
                        checkOutButton.Click += btnCheckOut_OnClick;
                        ApplicationBar.Buttons.Add(checkOutButton);
                    }
                     */
                    this.savedUserDisplayName.Value = colleague.FirstName;
                    this.savedUserLocation.Value = colleague.CurrentLocation;
                    this.savedUserLocationId.Value = colleague.CurrentLocationId;
                }
                //colleagueStr += string.Format("{0} {1}\n", colleague.Element("firstname").Value, colleague.Element("lastname").Value);
            }
            txtColleaguesLoading.Visibility = Visibility.Collapsed;

        }

        private void OnLocationServiceOpenReadComplete(object sender, OpenReadCompletedEventArgs e)
        {
            
            var reader = new StreamReader(e.Result);
            var result = reader.ReadToEnd();
            XmlReader XMLReader = XmlReader.Create(new MemoryStream(System.Text.UnicodeEncoding.Unicode.GetBytes(result)));
            XDocument xml = XDocument.Load(XMLReader);

            var xLocations = from loc in xml.Descendants("location")
                              select loc;
            foreach (var xLocation in xLocations)
            {
                var location = new Location
                {
                    LocationID = xLocation.Element("guid").Value,
                    Name = xLocation.Element("name").Value,
                    Address = xLocation.Element("address").Value
                };
                this.lstLocations.Items.Add(location);
                //colleagueStr += string.Format("{0} {1}\n", colleague.Element("firstname").Value, colleague.Element("lastname").Value);
            }
            
            /*
            var location = new Location
            {
                LocationID = "bca88939-68dc-467d-a354-d733c9a5cce2",
                Name = "Out of the office",
                Address = "n/a"
            };
            this.lstLocations.Items.Add(location);
            location = new Location
            {
                LocationID = "42ef27d8-a152-4651-befd-1a163ad0fabd",
                Name = "St Johns 4F Main",
                Address = "42 St John's Square"
            };
            this.lstLocations.Items.Add(location);
            location = new Location
            {
                LocationID = "771edc9a-f534-4a48-8bb6-a053dff155ec",
                Name = "Rosebery 2F Main",
                Address = "151 Rosebery Avenue"
            };
            this.lstLocations.Items.Add(location);
             */

            txtLocationsLoading.Visibility = Visibility.Collapsed;
        }

        private void lstColleagues_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstColleagues.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/ColleagueDetails.xaml?userName=" + this.lstColleagues.SelectedValue, UriKind.Relative));
            }
            //this.lstColleagues.SelectedIndex = -1;
        }

        private void lstLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstLocations.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/LocationDetails.xaml?locationId=" + this.lstLocations.SelectedValue, UriKind.Relative));
            }
            //this.lstColleagues.SelectedIndex = -1;
        }

        public void btnSettings_OnClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
        /*
        public void btnCheckOut_OnClick(object sender, EventArgs e)
        {
            locationClient = new WebClient();
            locationClient.OpenReadCompleted += OnCheckOutServiceOpenReadComplete;
            var url = "http://locator.rgahosting.com/LocatorService.svc/employees/" + this.savedUserName.Value + "?location=bca88939-68dc-467d-a354-d733c9a5cce2&date=" + DateTime.Now.ToString();
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
                    MessageBox.Show("You have been checked out.");
                    this.setUpApplicationBar();
                }
            }


        }
         */

    }
}