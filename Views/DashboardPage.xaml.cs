using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SoccerLink.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        private readonly DispatcherTimer _dateTimer;

        public DashboardPage()
        {
            InitializeComponent();

            UpdateHeaderDateTime();

            _dateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _dateTimer.Tick += (s, e) => UpdateHeaderDateTime();
            _dateTimer.Start();
        }

        private void UpdateHeaderDateTime()
        {
            HeaderText.Text = $"{DateTime.Now:D} {DateTime.Now:HH:mm}";
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new MessagesPage();
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new CalendarPage();
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new StatsNaviPage();
        }

        private void UpcomingEventButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new UpcomingEventPage();
        }

        private void NextEventButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Content = new NextEventPage();
        }

        private void TeamManagementButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new TeamManagementPage();
        }

    }
}
