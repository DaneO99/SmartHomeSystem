using System;
using System.Linq;
using Microsoft.Maui.Controls;
using SmartHomeApp.ViewModels;

namespace SmartHomeApp
{
    public partial class AppShell : Shell
    {
        // single, shared VM for all pages
        public static MainViewModel AppViewModel { get; } = new MainViewModel();

        public AppShell()
        {
            InitializeComponent();
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window != null)
            {
                window.Page = new NavigationPage(new Views.LoginPage());
            }
        }
    }
}
