using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    /// <summary>
    /// Checkout page — confirms shipping details, payment method, and places the order.
    /// Receives delivery date/time as navigation parameters from CartPage.
    /// </summary>
    [QueryProperty(nameof(ScheduledDelivery), "scheduledDelivery")]
    public partial class CheckoutPage : ContentPage
    {
        // ── Services ─────────────────────────────────────────────────────────
        private readonly CartService        _cartService    = CartService.Instance;
        private readonly UserProfileService _profileService = UserProfileService.Instance;
        private const decimal DeliveryFee = 2.99m;

        // ── State ────────────────────────────────────────────────────────────
        private string _selectedPaymentMethod = "Cash on Delivery";
        private DateTime _scheduledDelivery = DateTime.Now.AddDays(1);

        /// <summary>
        /// Query parameter: serialized delivery DateTime passed from CartPage.
        /// </summary>
        public string ScheduledDelivery
        {
            set
            {
                if (DateTime.TryParse(value, out DateTime dt))
                    _scheduledDelivery = dt;
            }
        }

        // ── Constructor ──────────────────────────────────────────────────────
        public CheckoutPage()
        {
            InitializeComponent();
        }

        // ── Lifecycle ────────────────────────────────────────────────────────
        protected override void OnAppearing()
        {
            base.OnAppearing();
            PopulateSummary();
            PreFillCustomerDetails();
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private void PopulateSummary()
        {
            OrderItemsView.ItemsSource = _cartService.CartItems.ToList();

            ScheduledLabel.Text = _scheduledDelivery.ToString("ddd, MMM d • hh:mm tt");

            decimal subtotal = _cartService.GetTotalAmount();
            decimal total    = subtotal + (_cartService.CartItems.Count > 0 ? DeliveryFee : 0m);

            SummarySubtotalLabel.Text    = $"${subtotal:F2}";
            SummaryDeliveryFeeLabel.Text = _cartService.CartItems.Count > 0 ? $"${DeliveryFee:F2}" : "$0.00";
            SummaryTotalLabel.Text       = $"${total:F2}";
        }

        private void PreFillCustomerDetails()
        {
            var p = _profileService.Profile;
            if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
                FullNameEntry.Text = p.FullName;

            if (string.IsNullOrWhiteSpace(ContactEntry.Text))
                ContactEntry.Text = p.ContactNumber;

            if (string.IsNullOrWhiteSpace(AddressEditor.Text))
                AddressEditor.Text = p.DefaultAddress;
        }

        private static async Task PressPopAsync(VisualElement el,
                                                double scale  = 0.92,
                                                uint   downMs = 45,
                                                uint   upMs   = 70)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0,   upMs,   Easing.SpringOut);
        }

        // ── Event handlers ───────────────────────────────────────────────────
        private void OnPaymentMethodChanged(object? sender, CheckedChangedEventArgs e)
        {
            try
            {
                if (!e.Value) return;

                if (sender is RadioButton rb && rb.Value is string method)
                {
                    _selectedPaymentMethod = method;
                    SelectedPaymentLabel.Text = $"Selected: {method}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Checkout] PaymentMethod error: {ex.Message}");
            }
        }

        private async void OnConfirmOrderClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 50, upMs: 70);

                // ── Validation ───────────────────────────────────────────────────
                string name    = FullNameEntry.Text?.Trim()    ?? string.Empty;
                string contact = ContactEntry.Text?.Trim()     ?? string.Empty;
                string address = AddressEditor.Text?.Trim()    ?? string.Empty;

                if (string.IsNullOrWhiteSpace(name))
                {
                    await DisplayAlertAsync("Missing Info", "Please enter your full name.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(contact))
                {
                    await DisplayAlertAsync("Missing Info", "Please enter your contact number.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(address))
                {
                    await DisplayAlertAsync("Missing Info", "Please enter your shipping address.", "OK");
                    return;
                }
                if (_cartService.CartItems.Count == 0)
                {
                    await DisplayAlertAsync("Empty Cart", "Your cart is empty. Add dishes before checkout.", "OK");
                    return;
                }

                // ── Build Order ──────────────────────────────────────────────────
                decimal total = _cartService.GetTotalAmount() + DeliveryFee;

                var order = new Order
                {
                    CustomerName          = name,
                    ContactNumber         = contact,
                    ShippingAddress       = address,
                    PaymentMethod         = _selectedPaymentMethod,
                    Items                 = [.. _cartService.CartItems],
                    TotalAmount           = total,
                    ScheduledDeliveryDate = _scheduledDelivery,
                    Status                = "Pending"
                };

                // Persist to shared in-memory store
                MockDataService.AddOrder(order);

                // Clear cart immediately on the UI thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _cartService.ClearCart();
                });

                // ── Success feedback ─────────────────────────────────────────────
                await DisplayAlertAsync(
                    "Order Placed Successfully!",
                    $"Your order has been dispatched.\n\nThank you, {name}!\n" +
                    $"Order #{order.Id} of ${total:F2} has been placed via {_selectedPaymentMethod}.\n" +
                    $"Scheduled for: {_scheduledDelivery:ddd, MMM d yyyy} at {_scheduledDelivery:hh:mm tt}\n\n" +
                    $"Delivering to: {address}",
                    "OK");

                // Navigate back to Catalog
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Checkout] ConfirmOrder error: {ex.Message}");
                await DisplayAlertAsync("Error", "Something went wrong placing your order. Please try again.", "OK");
            }
        }
    }
}
