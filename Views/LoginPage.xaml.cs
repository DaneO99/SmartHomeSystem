using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace SmartHomeApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            PasswordEntry.IsPassword = !e.Value;
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            var user = UsernameEntry.Text?.Trim();
            var pass = PasswordEntry.Text;

            if (user == "user" && pass == "password")
            {
                ErrorLabel.IsVisible = false;

                // Safely get the current window and swap its Page to the Shell
                var window = Application.Current?.Windows.FirstOrDefault();
                if (window != null)
                {
                    window.Page = new AppShell();
                }
            }
            else
            {
                ErrorLabel.Text = "Invalid credentials";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}