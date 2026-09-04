using System.Collections.Specialized;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    /// <summary>
    /// Cart review page — adjust quantities, apply promo codes, schedule delivery, and proceed to checkout.
    /// Hardware-accelerated with GPU ScaleToAsync() micro-interactions and dynamic state synchronization.
    /// </summary>
    public partial class CartPage : ContentPage
    {
        private readonly CartService _cartService = CartService.Instance;

        // Flat delivery fee
        private const decimal BaseDeliveryFee = 2.99m;

        // Promo discount state
        private decimal _discountAmount = 0m;
        private string  _appliedPromoCode = string.Empty;

        // Design-system colours
        private static readonly Color ColBrandGreen   = Color.FromArgb("#FF6B4A"); // Coral
        private static readonly Color ColDisabled     = Color.FromArgb("#566070");

        public CartPage()
        {
            InitializeComponent();
            _cartService.CartItems.CollectionChanged += OnCartCollectionChanged;
        }

        private void OnCartCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(RefreshCart);
        }

        // ── Lifecycle ────────────────────────────────────────────────────────
        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshCart();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>Re-binds the collection view and recalculates summary labels dynamically.</summary>
        private void RefreshCart()
        {
            try
            {
                var items = _cartService.CartItems.ToList();
                CartCollectionView.ItemsSource = null;
                CartCollectionView.ItemsSource = items;

                bool    hasItems = items.Count > 0;
                decimal subtotal = _cartService.GetTotalAmount();
                decimal fee      = hasItems ? (_appliedPromoCode == "FREESHIP" ? 0m : BaseDeliveryFee) : 0m;

                if (!hasItems)
                {
                    _discountAmount = 0m;
                    _appliedPromoCode = string.Empty;
                    if (PromoStatusLabel != null) PromoStatusLabel.IsVisible = false;
                }
                else if (_appliedPromoCode == "SAVE20")
                {
                    _discountAmount = subtotal * 0.20m;
                }
                else if (_appliedPromoCode == "FEAST10")
                {
                    _discountAmount = Math.Min(subtotal, 10.00m);
                }

                decimal total = Math.Max(0m, subtotal + fee - _discountAmount);

                SubtotalLabel.Text    = subtotal.ToString("C");
                DeliveryFeeLabel.Text = hasItems ? (fee == 0m ? "FREE (Promo)" : fee.ToString("C")) : "FREE";
                TotalLabel.Text       = total.ToString("C");

                if (DiscountRow != null && DiscountLabel != null)
                {
                    if (_discountAmount > 0m)
                    {
                        DiscountRow.IsVisible = true;
                        DiscountLabel.Text = $"-{_discountAmount:C}";
                    }
                    else
                    {
                        DiscountRow.IsVisible = false;
                    }
                }

                // Checkout button state
                CheckoutButton.IsEnabled       = hasItems;
                CheckoutButton.BackgroundColor = hasItems ? ColBrandGreen : ColDisabled;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] RefreshCart error: {ex.Message}");
            }
        }

        // ── Shared animation helper ───────────────────────────────────────────

        private static async Task PressPopAsync(VisualElement el,
                                                double scale  = 0.90,
                                                uint   downMs = 45,
                                                uint   upMs   = 70)
        {
            await el.ScaleToAsync(scale, downMs,  Easing.CubicIn);
            await el.ScaleToAsync(1.0,   upMs,    Easing.SpringOut);
        }

        // ── Empty-state navigation ────────────────────────────────────

        private async void OnBrowseProductsClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] BrowseProducts nav error: {ex.Message}");
            }
        }

        // ── Promo Code Handler ───────────────────────────────────────────────

        private async void OnApplyPromoClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);

                string code = PromoCodeEntry?.Text?.Trim().ToUpperInvariant() ?? string.Empty;

                if (string.IsNullOrEmpty(code))
                {
                    await DisplayAlertAsync("Promo Code", "Please enter a valid promo code.", "OK");
                    return;
                }

                decimal subtotal = _cartService.GetTotalAmount();
                if (subtotal == 0m)
                {
                    await DisplayAlertAsync("Empty Cart", "Add items to your cart before applying a promo code.", "OK");
                    return;
                }

                if (code == "SAVE20")
                {
                    _discountAmount = subtotal * 0.20m;
                    _appliedPromoCode = code;
                    PromoStatusLabel.Text = $"🎉 'SAVE20' Applied: 20% Off (-{_discountAmount:C})";
                    PromoStatusLabel.TextColor = Color.FromArgb("#2ECC71");
                    PromoStatusLabel.IsVisible = true;
                }
                else if (code == "FREESHIP")
                {
                    _discountAmount = 0m;
                    _appliedPromoCode = code;
                    PromoStatusLabel.Text = "🚚 'FREESHIP' Applied: 100% Free Delivery!";
                    PromoStatusLabel.TextColor = Color.FromArgb("#2ECC71");
                    PromoStatusLabel.IsVisible = true;
                }
                else if (code == "FEAST10")
                {
                    _discountAmount = Math.Min(subtotal, 10.00m);
                    _appliedPromoCode = code;
                    PromoStatusLabel.Text = $"🔥 'FEAST10' Applied: ${_discountAmount:F2} Off!";
                    PromoStatusLabel.TextColor = Color.FromArgb("#2ECC71");
                    PromoStatusLabel.IsVisible = true;
                }
                else
                {
                    PromoStatusLabel.Text = "❌ Invalid promo code. Try 'SAVE20', 'FREESHIP', or 'FEAST10'.";
                    PromoStatusLabel.TextColor = Color.FromArgb("#EF4444");
                    PromoStatusLabel.IsVisible = true;
                    return;
                }

                RefreshCart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] Promo code error: {ex.Message}");
            }
        }

        // ── Cart item event handlers ─────────────────────────────────────────

        private async void OnIncrementClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is CartItem item)
                {
                    await PressPopAsync(btn, scale: 0.85, downMs: 40, upMs: 65);
                    _cartService.IncrementQuantity(item);
                    RefreshCart();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] Increment error: {ex.Message}");
            }
        }

        private async void OnDecrementClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is CartItem item)
                {
                    await PressPopAsync(btn, scale: 0.85, downMs: 40, upMs: 65);
                    _cartService.DecrementQuantity(item);
                    RefreshCart();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] Decrement error: {ex.Message}");
            }
        }

        private async void OnRemoveClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is CartItem item)
                {
                    await PressPopAsync(btn, scale: 0.85, downMs: 45, upMs: 70);

                    bool confirm = await DisplayAlertAsync(
                        "Remove Item",
                        $"Remove \"{item.Product?.Name}\" from your cart?",
                        "Remove", "Cancel");

                    if (confirm)
                    {
                        _cartService.RemoveFromCart(item);
                        RefreshCart();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] Remove error: {ex.Message}");
            }
        }

        // ── Checkout & Place Order handlers ─────────────────────────────────

        /// <summary>
        /// Direct Place Order handler that validates, records the order, empties the cart,
        /// and resets the view back to the catalog.
        /// </summary>
        public async void OnPlaceOrderClicked(object? sender, EventArgs e)
        {
            try
            {
                if (_cartService.CartItems.Count == 0)
                {
                    await DisplayAlertAsync("Empty Cart", "Your cart is empty. Please add dishes before placing an order.", "OK");
                    return;
                }

                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 50);

                DateTime selectedDate = DeliveryDatePicker?.Date ?? DateTime.Today;
                DateTime scheduledAt = selectedDate.Date + (DeliveryTimePicker?.Time ?? TimeSpan.Zero);

                if (scheduledAt < DateTime.Now)
                {
                    await DisplayAlertAsync(
                        "Invalid Time",
                        "Please select a future date and time for your delivery.",
                        "OK");
                    return;
                }

                // Calculate totals
                decimal subtotal = _cartService.GetTotalAmount();
                decimal fee = _appliedPromoCode == "FREESHIP" ? 0m : BaseDeliveryFee;
                decimal total = Math.Max(0m, subtotal + fee - _discountAmount);

                var profile = UserProfileService.Instance.Profile;

                // Create and persist order
                var order = new Order
                {
                    CustomerName          = !string.IsNullOrWhiteSpace(profile?.FullName) ? profile.FullName : "Guest Customer",
                    ContactNumber         = !string.IsNullOrWhiteSpace(profile?.ContactNumber) ? profile.ContactNumber : "N/A",
                    ShippingAddress       = !string.IsNullOrWhiteSpace(profile?.DefaultAddress) ? profile.DefaultAddress : "Standard Delivery Address",
                    PaymentMethod         = "Cash on Delivery",
                    Items                 = [.. _cartService.CartItems],
                    TotalAmount           = total,
                    ScheduledDeliveryDate = scheduledAt,
                    Status                = "Pending"
                };

                MockDataService.AddOrder(order);

                // Clear cart immediately on the UI thread and refresh calculations
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _cartService.ClearCart();
                    RefreshCart();
                });

                // Display success confirmation modal
                await DisplayAlertAsync(
                    "Order Placed Successfully!",
                    "Your order has been dispatched.",
                    "OK");

                // Automatically redirect back to Catalog
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] PlaceOrder error: {ex.Message}");
                await DisplayAlertAsync("Error", "Something went wrong placing your order. Please try again.", "OK");
            }
        }

        private async void OnCheckoutClicked(object? sender, EventArgs e)
        {
            try
            {
                if (_cartService.CartItems.Count == 0)
                {
                    await DisplayAlertAsync("Empty Cart", "Please add items before checking out.", "OK");
                    return;
                }

                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 50);

                DateTime selectedDate = DeliveryDatePicker?.Date ?? DateTime.Today;
                DateTime scheduledAt = selectedDate.Date + (DeliveryTimePicker?.Time ?? TimeSpan.Zero);

                if (scheduledAt < DateTime.Now)
                {
                    await DisplayAlertAsync(
                        "Invalid Time",
                        "Please select a future date and time for your delivery.",
                        "OK");
                    return;
                }

                string encoded = Uri.EscapeDataString(scheduledAt.ToString("O"));
                await Shell.Current.GoToAsync(
                    $"{nameof(CheckoutPage)}?scheduledDelivery={encoded}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartPage] Checkout error: {ex.Message}");
                await DisplayAlertAsync("Error", "Something went wrong. Please try again.", "OK");
            }
        }
    }
}
