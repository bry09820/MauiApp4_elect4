using Microsoft.Extensions.DependencyInjection;

namespace MauiApp4_elect4
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Show LandingPage first; it swaps to AppShell on "Get Started"
            return new Window(new Views.LandingPage());
        }
    }
}