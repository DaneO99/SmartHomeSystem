using System;
using System.Linq;
using Microsoft.Maui.Controls;
using SmartHomeApp.ViewModels;

namespace SmartHomeApp
{
    /// <summary>
    /// Hosts the flyout menu and provides the single shared MainViewModel.
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// Single shared ViewModel instance for all pages.
        /// </summary>
        public static MainViewModel AppViewModel { get; } = new MainViewModel();

        public AppShell()
        {
            InitializeComponent();
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Stop background scheduler
            AppViewModel.StopScheduler();

            // Navigate back to login
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window != null)
            {
                window.Page = new NavigationPage(new Views.LoginPage());
            }
        }
    }
}
