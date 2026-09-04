using MauiApp4_elect4.Models;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    public partial class MyCartPage : ContentPage
    {
        public CartViewModel ViewModel { get; }

        public MyCartPage()
        {
            InitializeComponent();
            ViewModel = new CartViewModel();
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.NotifyCalculationsChanged();
        }

        private static async Task PressPopAsync(VisualElement el, double scale = 0.90, uint downMs = 40, uint upMs = 60)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0, upMs, Easing.SpringOut);
        }

        private async void OnBackTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync("..");
            }
            catch
            {
                await Shell.Current.GoToAsync("//ExploreShopsPage");
            }
        }

        private async void OnExploreShopsClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync("//ExploreShopsPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MyCartPage] Explore nav error: {ex.Message}");
            }
        }

        private async void OnIncrementTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, 0.85);

                if (e.Parameter is CartItem item)
                {
                    ViewModel.IncrementQuantity(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MyCartPage] Increment error: {ex.Message}");
            }
        }

        private async void OnDecrementTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, 0.85);

                if (e.Parameter is CartItem item)
                {
                    ViewModel.DecrementQuantity(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MyCartPage] Decrement error: {ex.Message}");
            }
        }

        private async void OnApplyPromoClicked(object? sender, EventArgs e)
        {
            try
            {
                string code = (PromoCodeEntry?.Text ?? string.Empty).Trim();
                bool success = ViewModel.ApplyPromoCode(code, out string message);

                if (success)
                {
                    await DisplayAlertAsync("Promo Applied! 🎉", message, "Awesome");
                }
                else
                {
                    await DisplayAlertAsync("Promo Code", message, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MyCartPage] Promo error: {ex.Message}");
            }
        }

        private async void OnProceedToCheckoutClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, 0.95);

                if (!ViewModel.HasItems)
                {
                    await DisplayAlertAsync("Empty Cart", "Please add items to your cart before proceeding.", "OK");
                    return;
                }

                await Shell.Current.GoToAsync(nameof(CheckoutPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MyCartPage] Checkout nav error: {ex.Message}");
            }
        }
    }
}
