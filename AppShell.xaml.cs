// Imports for navigation, shell control, and view model reference
using System;                                    // Provides basic system types
using System.Linq;                               // Provides LINQ extension methods
using Microsoft.Maui.Controls;                   // Provides Shell and navigation interfaces
using SmartHomeApp.ViewModels;                   // Provides MainViewModel for shared data

namespace SmartHomeApp
{
    /// <summary>
    /// Application shell defining overall navigation structure and shared resources.
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// Single, shared view model instance for use across all pages.
        /// </summary>
        public static MainViewModel AppViewModel { get; } = new MainViewModel();

        /// <summary>
        /// Constructs the application shell and initializes its components.
        /// </summary>
        public AppShell()
        {
            InitializeComponent();  // Load XAML-defined shell layout and routes
        }

        /// <summary>
        /// Handles the logout menu item click event.
        /// Navigates to the login page by replacing the current window's root.
        /// </summary>
        /// <param name="sender">MenuItem that triggered the event.</param>
        /// <param name="e">Event arguments (unused).</param>
        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Retrieve the first application window
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window != null)
            {
                // Replace root page with a navigation stack containing the login page
                window.Page = new NavigationPage(new Views.LoginPage());
            }
        }
    }
}
