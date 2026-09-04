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
            // Launch directly into AppShell displaying the forest-green views
            return new Window(new AppShell());
        }
    }
}