﻿#pragma checksum "C:\Users\DannyLee\Documents\Visual Studio 2010\Projects\VirtualReceptionist\VirtualReceptionist\ColleagueDetails.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "95402B47B8DFCE8D5EE9FDCB6A34DAB6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace VirtualReceptionist {
    
    
    public partial class ColleagueDetails : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.TextBlock txtIntro;
        
        internal System.Windows.Controls.TextBlock txtFirstName;
        
        internal System.Windows.Controls.TextBlock txtLastName;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btnSettings;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/VirtualReceptionist;component/ColleagueDetails.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.txtIntro = ((System.Windows.Controls.TextBlock)(this.FindName("txtIntro")));
            this.txtFirstName = ((System.Windows.Controls.TextBlock)(this.FindName("txtFirstName")));
            this.txtLastName = ((System.Windows.Controls.TextBlock)(this.FindName("txtLastName")));
            this.btnSettings = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btnSettings")));
        }
    }
}
