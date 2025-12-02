using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.ViewModels;
using System;

namespace SoccerLink.Views
{
    public sealed partial class MessagesPage : Page
    {
        public MessagesViewModel ViewModel { get; }

        public MessagesPage()
        {
            ViewModel = new MessagesViewModel();
            this.InitializeComponent();
            this.Loaded += MessagesPage_Loaded;
        }

        private async void MessagesPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadMessagesAsync();
        }

        private void MessagesFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;
            var selectedTag = (MessagesFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            if (selectedTag != null)
            {
                ViewModel.ApplyFilter(selectedTag);
            }
        }

        private void MessagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MessagesList.SelectedItem is Wiadomosc msg)
            {
                ViewModel.SelectedMessage = msg;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DashboardPage));
        }

        private void NewMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewMessagePage));
        }
    }
}