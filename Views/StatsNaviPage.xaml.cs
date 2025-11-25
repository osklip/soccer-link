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
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SoccerLink.Views
{
    public sealed partial class StatsNaviPage : Page
    {
        public StatsNaviPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new DashboardPage();
        }

        private void SoloStatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new StatsPlayerPage();
        }

        private void TeamStatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new StatsTeamPage();
        }

        // NOWA METODA
        private void AddStatsButton_Click(object sender, RoutedEventArgs e)
        {
            // Nawigacja do strony wyboru meczu
            this.Content = new SelectMatchPage();
        }
    }
}