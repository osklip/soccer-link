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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SoccerLink.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewMessagePage : Page
    {
        public NewMessagePage()
        {
            InitializeComponent();
            SendButton.Click += SendButton_Click; // Przycisk wyœle w przysz³oœci wiadomoœæ
            SendAllButton.Click += SendAllButton_Click; // Przycisk do wysy³ki grupowej
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Powrót do MessagesPage
            this.Content = new MessagesPage();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // StatusTextBlock.Text = "TO DO: Implementacja wysy³ania wiadomoœci";
        }

        private void SendAllButton_Click(object sender, RoutedEventArgs e)
        {
            // StatusTextBlock.Text = "TO DO: Logika wysy³ki do wszystkich";
        }
    }
}
