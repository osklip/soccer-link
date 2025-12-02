using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels;
using System;

namespace SoccerLink.Views
{
    public sealed partial class NewMessagePage : Page
    {
        public NewMessageViewModel ViewModel { get; }

        public NewMessagePage()
        {
            ViewModel = new NewMessageViewModel();
            this.InitializeComponent();

            // Nawigacja powrotna
            ViewModel.RequestNavigateBack += (s, e) =>
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
                else this.Frame.Navigate(typeof(MessagesPage));
            };

            this.Loaded += NewMessagePage_Loaded;
        }

        private async void NewMessagePage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadRecipientsAsync();
        }
    }
}