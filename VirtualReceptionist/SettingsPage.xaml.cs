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

using System.Windows.Navigation;
using VirtualReceptionist.BusinessObjects;

namespace VirtualReceptionist
{

    public partial class SettingsPage : PhoneApplicationPage
    {
        Setting<string> savedUserName = new Setting<string>("UserName", "");

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Restore saved text box contents when entering this page
            txtUserName.Text = this.savedUserName.Value;
        }

        void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text so it can be cleared with one keystroke
            (sender as TextBox).SelectAll();

            
        }

        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == this.txtUserName)
                {
                    this.txtUserName.Focus();
                }
                e.Handled = true;
            }
        }

        public void btnCancel_OnClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        public void btnSave_OnClick(object sender, EventArgs e)
        {
            this.savedUserName.Value = txtUserName.Text;
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}