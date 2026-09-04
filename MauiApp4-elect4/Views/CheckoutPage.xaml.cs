using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    [QueryProperty(nameof(ScheduledDelivery), "scheduledDelivery")]
    public partial class CheckoutPage : ContentPage
    {
        private readonly CartService _cartService = CartService.Instance;
        private readonly UserProfileService _profileService = UserProfileService.Instance;

        private string _selectedDeliveryMethod = "Home Delivery";
        private string _selectedPaymentMethod = "Credit Card";
        private decimal _discountAmount = 0m;
        private const decimal BaseDeliveryFee = 1.09m;

        public string ScheduledDelivery { get; set; } = string.Empty;

        public CheckoutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCheckoutState();
        }

        private void LoadCheckoutState()
        {
            try
            {
                var profile = _profileService.Profile;
                if (!string.IsNullOrWhiteSpace(profile?.DefaultAddress))
                {
                    AddressDisplayLabel.Text = profile.DefaultAddress;
                }

                CalculateTotal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CheckoutPage] LoadState error: {ex.Message}");
            }
        }

        private void CalculateTotal()
        {
            decimal subtotal = _cartService.GetTotalAmount();
            decimal fee = _selectedDeliveryMethod == "Home Delivery" ? BaseDeliveryFee : 0m;
            decimal total = Math.Max(0m, subtotal + fee - _discountAmount);

            SummaryTotalLabel.Text = total.ToString("C");
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
                    AddressDisplayLabel.Text = action.Replace("📍 ", "").Replace("🏢 ", "").Replace("🏠 ", "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CheckoutPage] AddressChange error: {ex.Message}");
            }
        }

        private void OnHomeDeliverySelected(object? sender, TappedEventArgs e)
        {
            _selectedDeliveryMethod = "Home Delivery";

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

            CalculateTotal();
        }

        private void OnPickupSelected(object? sender, TappedEventArgs e)
        {
            _selectedDeliveryMethod = "Pickup";

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

            CalculateTotal();
        }

        private void OnCreditCardSelected(object? sender, TappedEventArgs e)
        {
            _selectedPaymentMethod = "Credit Card";

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
            _selectedPaymentMethod = "Digital Wallet";

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

                string code = CheckoutPromoEntry?.Text?.Trim().ToUpperInvariant() ?? string.Empty;
                decimal subtotal = _cartService.GetTotalAmount();

                if (code == "SAVE20")
                {
                    _discountAmount = subtotal * 0.20m;
                    PromoFeedbackLabel.Text = $"🎉 'SAVE20' Applied: -{_discountAmount:C}";
                    PromoFeedbackLabel.TextColor = Color.FromArgb("#1E6B39");
                    PromoFeedbackLabel.IsVisible = true;
                }
                else if (code == "FREESHIP")
                {
                    _discountAmount = BaseDeliveryFee;
                    PromoFeedbackLabel.Text = "🚚 'FREESHIP' Applied: Free Delivery";
                    PromoFeedbackLabel.TextColor = Color.FromArgb("#1E6B39");
                    PromoFeedbackLabel.IsVisible = true;
                }
                else
                {
                    _discountAmount = 0m;
                    PromoFeedbackLabel.Text = "❌ Invalid promo code.";
                    PromoFeedbackLabel.TextColor = Color.FromArgb("#EF4444");
                    PromoFeedbackLabel.IsVisible = true;
                }

                CalculateTotal();
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

                decimal subtotal = _cartService.GetTotalAmount();
                decimal fee = _selectedDeliveryMethod == "Home Delivery" ? BaseDeliveryFee : 0m;
                decimal total = Math.Max(0m, subtotal + fee - _discountAmount);

                var profile = _profileService.Profile;

                var order = new Order
                {
                    CustomerName = !string.IsNullOrWhiteSpace(profile?.FullName) ? profile.FullName : "Alex Rivera",
                    ContactNumber = !string.IsNullOrWhiteSpace(profile?.ContactNumber) ? profile.ContactNumber : "+1 (555) 019-2834",
                    ShippingAddress = AddressDisplayLabel.Text,
                    DeliveryMethod = _selectedDeliveryMethod,
                    PaymentMethod = _selectedPaymentMethod,
                    Items = [.. _cartService.CartItems],
                    Subtotal = subtotal > 0 ? subtotal : 11.99m,
                    DeliveryFee = fee,
                    DiscountAmount = _discountAmount,
                    TotalAmount = total > 0 ? total : 13.08m,
                    ScheduledDeliveryDate = DateTime.Now.AddMinutes(15),
                    EstimatedMinutes = 15,
                    CourierName = "Mike Roberts",
                    CourierPhone = "+1 (555) 839-2041",
                    Status = "Out for Delivery"
                };

                MockDataService.AddOrder(order);

                // Clear active cart
                _cartService.ClearCart();

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
