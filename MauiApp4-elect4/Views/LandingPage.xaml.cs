using Microsoft.Maui.Controls;

namespace MauiApp4_elect4.Views
{
    public partial class LandingPage : ContentPage
    {
        private bool _navigating = false;

        public LandingPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (ContentStack != null)
                {
                    ContentStack.Opacity = 0;
                    ContentStack.Scale = 0.92;

                    await Task.WhenAll(
                        ContentStack.FadeToAsync(1.0, 200, Easing.CubicOut),
                        ContentStack.ScaleToAsync(1.0, 180, Easing.CubicOut)
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LandingPage] Entrance error: {ex.Message}");
            }
        }

        private async void OnGetStartedClicked(object? sender, EventArgs e)
        {
            if (_navigating) return;
            _navigating = true;

            try
            {
                // Instant tactile micro-feedback
                if (sender is VisualElement btn)
                {
                    await btn.ScaleToAsync(0.95, 30, Easing.CubicIn);
                    _ = btn.ScaleToAsync(1.0, 30, Easing.CubicOut);
                }

                // Direct & reliable Main Thread Page switch via modern Window Page
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current != null && Application.Current.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new AppShell();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LandingPage] Navigation error: {ex.Message}");
                _navigating = false;
            }
        }
    }
}