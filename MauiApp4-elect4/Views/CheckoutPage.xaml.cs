using MauiApp4_elect4.Models;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    [QueryProperty(nameof(ScheduledDelivery), "scheduledDelivery")]
    public partial class CheckoutPage : ContentPage
    {
        public CheckoutViewModel ViewModel { get; }

        public string ScheduledDelivery { get; set; } = string.Empty;

        public CheckoutPage()
        {
            InitializeComponent();
            ViewModel = new CheckoutViewModel();
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadState();
            UpdateUiFromViewModel();
        }

        private void UpdateUiFromViewModel()
        {
            AddressDisplayLabel.Text = ViewModel.ShippingAddress;
            SummaryTotalLabel.Text = ViewModel.FormattedTotal;
        }

        private static async Task PressPopAsync(VisualElement el, double scale = 0.92, uint downMs = 45, uint upMs = 65)
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

        private async void OnChangeAddressTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                string action = await DisplayActionSheetAsync(
                    "📍 Select Delivery Address",
                    "Cancel",
                    null,
                    "📍 742 Evergreen Terrace, Springfield, OR 97477",
                    "🏢 Office: 100 Market St, Suite 400",
                    "🏠 Home: 25 Green Valley Road, Unit 4B",
                    "🛰️ Use Current GPS Location");

                if (!string.IsNullOrEmpty(action) && action != "Cancel")
                {
                    string cleanedAddress = action.Replace("📍 ", "").Replace("🏢 ", "").Replace("🏠 ", "");
                    ViewModel.SetAddress(cleanedAddress);
                    AddressDisplayLabel.Text = cleanedAddress;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CheckoutPage] AddressChange error: {ex.Message}");
            }
        }

        private void OnHomeDeliverySelected(object? sender, TappedEventArgs e)
        {
            ViewModel.SetDeliveryMethod("Home Delivery");

            HomeDeliveryCard.Stroke = Color.FromArgb("#1E6B39");
            HomeDeliveryCard.StrokeThickness = 1.5;
            HomeDeliveryCheckBorder.BackgroundColor = Color.FromArgb("#1E6B39");
            HomeDeliveryCheckBorder.StrokeThickness = 0;
            HomeDeliveryCheckMark.Text = "✓";

            PickupCard.Stroke = Color.FromArgb("#E8ECF2");
            PickupCard.StrokeThickness = 1;
            PickupCheckBorder.BackgroundColor = Colors.White;
            PickupCheckBorder.StrokeThickness = 1.5;
            PickupCheckMark.Text = "";

            SummaryTotalLabel.Text = ViewModel.FormattedTotal;
        }

        private void OnPickupSelected(object? sender, TappedEventArgs e)
        {
            ViewModel.SetDeliveryMethod("Pickup");

            PickupCard.Stroke = Color.FromArgb("#1E6B39");
            PickupCard.StrokeThickness = 1.5;
            PickupCheckBorder.BackgroundColor = Color.FromArgb("#1E6B39");
            PickupCheckBorder.StrokeThickness = 0;
            PickupCheckMark.Text = "✓";

            HomeDeliveryCard.Stroke = Color.FromArgb("#E8ECF2");
            HomeDeliveryCard.StrokeThickness = 1;
            HomeDeliveryCheckBorder.BackgroundColor = Colors.White;
            HomeDeliveryCheckBorder.StrokeThickness = 1.5;
            HomeDeliveryCheckMark.Text = "";

            SummaryTotalLabel.Text = ViewModel.FormattedTotal;
        }

        private void OnCreditCardSelected(object? sender, TappedEventArgs e)
        {
            ViewModel.SetPaymentMethod("Credit Card");

            CreditCardBorder.Stroke = Color.FromArgb("#1E6B39");
            CreditCardBorder.StrokeThickness = 1.5;
            CreditCardCheckBorder.BackgroundColor = Color.FromArgb("#1E6B39");
            CreditCardCheckBorder.StrokeThickness = 0;
            CreditCardCheckMark.Text = "✓";

            WalletBorder.Stroke = Color.FromArgb("#E8ECF2");
            WalletBorder.StrokeThickness = 1;
            WalletCheckBorder.BackgroundColor = Colors.White;
            WalletCheckBorder.StrokeThickness = 1.5;
            WalletCheckMark.Text = "";
        }

        private void OnWalletSelected(object? sender, TappedEventArgs e)
        {
            ViewModel.SetPaymentMethod("Digital Wallet");

            WalletBorder.Stroke = Color.FromArgb("#1E6B39");
            WalletBorder.StrokeThickness = 1.5;
            WalletCheckBorder.BackgroundColor = Color.FromArgb("#1E6B39");
            WalletCheckBorder.StrokeThickness = 0;
            WalletCheckMark.Text = "✓";

            CreditCardBorder.Stroke = Color.FromArgb("#E8ECF2");
            CreditCardBorder.StrokeThickness = 1;
            CreditCardCheckBorder.BackgroundColor = Colors.White;
            CreditCardCheckBorder.StrokeThickness = 1.5;
            CreditCardCheckMark.Text = "";
        }

        private async void OnApplyPromoClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 35, upMs: 55);

                string code = CheckoutPromoEntry?.Text?.Trim() ?? string.Empty;
                bool success = ViewModel.ApplyPromoCode(code);

                PromoFeedbackLabel.Text = ViewModel.PromoFeedbackText;
                PromoFeedbackLabel.TextColor = success ? Color.FromArgb("#1E6B39") : Color.FromArgb("#EF4444");
                PromoFeedbackLabel.IsVisible = ViewModel.IsPromoFeedbackVisible;

                SummaryTotalLabel.Text = ViewModel.FormattedTotal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CheckoutPage] Promo error: {ex.Message}");
            }
        }

        private async void OnPlaceOrderClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 50);

                // Place order and automatically reset the active cart
                Order order = await ViewModel.PlaceOrderAsync();

                // Navigate to OrderCompletedPage
                await Shell.Current.GoToAsync($"{nameof(OrderCompletedPage)}?orderId={order.Id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CheckoutPage] PlaceOrder error: {ex.Message}");
                await Shell.Current.GoToAsync(nameof(OrderCompletedPage));
            }
        }
    }
}
