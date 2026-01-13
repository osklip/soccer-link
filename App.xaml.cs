using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Views;
using System;

namespace SoccerLink
{
    public partial class App : Application
    {
        // 1. ZMIEŃ: Dodaj publiczną właściwość
        public Window MainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 2. ZMIEŃ: Przypisz nowe okno do publicznej właściwości
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}