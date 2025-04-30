using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace SmartHomeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // Note the '?' on IActivationState to match the base signature
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
