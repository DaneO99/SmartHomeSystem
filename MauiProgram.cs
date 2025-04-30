// Imports for MAUI Community Toolkit integration and logging configuration
using CommunityToolkit.Maui;      // Provides additional helpers and controls for MAUI
using Microsoft.Extensions.Logging; // Provides logging interfaces and extensions

namespace SmartHomeApp
{
    /// <summary>
    /// Configures and creates the MAUI application instance,
    /// registering services, fonts, and community toolkit extensions.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Builds and returns a configured <see cref="MauiApp"/>.
        /// </summary>
        /// <returns>Configured MAUI application.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Register the core App class and CommunityToolkit extensions
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()      // Register CommunityToolkit.Maui services
                .ConfigureFonts(fonts =>
                {
                    // Add custom fonts for application styling
                    fonts.AddFont("OpenSans-Regular.ttf",   "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            // Enable debug logging in development builds
            builder.Logging.AddDebug();
#endif

            // Build and return the configured MAUI application
            return builder.Build();
        }
    }
}
