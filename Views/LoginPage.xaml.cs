// Namespace imports for UI controls and LINQ utilities
using System;                                    // Provides basic system types
using System.Linq;                               // Provides LINQ extension methods
using Microsoft.Maui.Controls;                   // Provides MAUI UI elements and events

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code-behind for LoginPage.xaml, handling user authentication
    /// and password visibility toggle.
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        /// <summary>
        /// Constructs page and initializes XAML-defined components.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();  // Load UI elements defined in XAML
        }

        /// <summary>
        /// Toggles password masking when the show-password checkbox changes state.
        /// </summary>
        /// <param name="sender">Checkbox that raised the event.</param>
        /// <param name="e">Event arguments containing the new checked value.</param>
        private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Flip IsPassword to show or hide plain text
            PasswordEntry.IsPassword = !e.Value;
        }

        /// <summary>
        /// Validates credentials against hardcoded values and navigates to AppShell on success.
        /// Displays error message on failure.
        /// </summary>
        /// <param name="sender">Button that raised the click event.</param>
        /// <param name="e">Event arguments (unused).</param>
        private void OnLoginClicked(object sender, EventArgs e)
        {
            var user = UsernameEntry.Text?.Trim();  // Retrieve and trim username input
            var pass = PasswordEntry.Text;          // Retrieve password input

            // Hardcoded credentials: user/password
            if (user == "user" && pass == "password")
            {
                ErrorLabel.IsVisible = false;       // Hide error label when credentials match

                // Swap current window's page to the application shell
                var window = Application.Current?.Windows.FirstOrDefault();
                if (window != null)
                {
                    window.Page = new AppShell();
                }
            }
            else
            {
                // Display error message for invalid credentials
                ErrorLabel.Text = "Invalid credentials";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}
