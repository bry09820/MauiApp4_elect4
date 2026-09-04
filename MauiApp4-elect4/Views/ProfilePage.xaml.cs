using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    /// <summary>
    /// Customer profile page — interactive account management, address management,
    /// payment options, dark mode toggle, help center, logout, and order history tracking.
    /// Hardware-accelerated with GPU ScaleToAsync() micro-interactions and dynamic state synchronization.
    /// </summary>
    public partial class ProfilePage : ContentPage
    {
        private readonly UserProfileService _profileService = UserProfileService.Instance;

        public ProfilePage()
        {
            InitializeComponent();
        }

        // ── Lifecycle ────────────────────────────────────────────────────────
        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                PopulateProfile();
                LoadOrderHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] OnAppearing error: {ex.Message}");
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private void PopulateProfile()
        {
            var p = _profileService.Profile;

            // Header avatar — first letter of first name
            string initials = string.IsNullOrWhiteSpace(p.FullName)
                ? "?" : p.FullName.Trim()[0].ToString().ToUpperInvariant();

            AvatarLabel.Text       = initials;
            ProfileNameLabel.Text  = p.FullName;
            ProfileEmailLabel.Text = p.Email;

            // Editable fields
            NameEntry.Text    = p.FullName;
            EmailEntry.Text   = p.Email;
            ContactEntry.Text = p.ContactNumber;
            AddressEditor.Text = p.DefaultAddress;

            if (DarkModeSwitch != null)
            {
                DarkModeSwitch.IsToggled = p.IsDarkMode;
            }
        }

        private void LoadOrderHistory()
        {
            var orders = _profileService.GetMyOrders();

            bool hasOrders = orders.Count > 0;
            NoOrdersLabel.IsVisible   = !hasOrders;
            OrderHistoryView.IsVisible = hasOrders;

            OrderHistoryView.ItemsSource = null;
            OrderHistoryView.ItemsSource = orders;
        }

        private static async Task PressPopAsync(VisualElement el,
                                                double scale  = 0.90,
                                                uint   downMs = 45,
                                                uint   upMs   = 70)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0,   upMs,   Easing.SpringOut);
        }

        // ── Interactive Shortcut Handlers ────────────────────────────────────

        /// <summary>Smoothly scrolls down to the Recent Orders section with tactile feedback.</summary>
        private async void OnOrdersShortcutClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);

                var orders = _profileService.GetMyOrders();
                if (orders.Count == 0)
                {
                    await DisplayAlertAsync("Orders History", "You have not placed any orders yet. Start exploring from the Catalog!", "OK");
                    return;
                }

                if (ProfileScrollView != null && OrdersSectionContainer != null)
                {
                    await ProfileScrollView.ScrollToAsync(OrdersSectionContainer, ScrollToPosition.Start, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] OrdersShortcut error: {ex.Message}");
            }
        }

        /// <summary>Opens address management modal/action sheet to choose or add addresses.</summary>
        private async void OnAddressesShortcutClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);

                var p = _profileService.Profile;
                var addressOptions = p.SavedAddresses.ToList();
                addressOptions.Add("➕ Add New Delivery Address");

                string action = await DisplayActionSheetAsync(
                    "📍 Manage Delivery Addresses",
                    "Cancel",
                    null,
                    addressOptions.ToArray());

                if (string.IsNullOrEmpty(action) || action == "Cancel") return;

                if (action == "➕ Add New Delivery Address")
                {
                    string? newAddress = await DisplayPromptAsync(
                        "New Address",
                        "Enter your complete delivery address:",
                        "Save",
                        "Cancel",
                        placeholder: "Unit / House No., Street, City");

                    if (!string.IsNullOrWhiteSpace(newAddress))
                    {
                        p.SavedAddresses.Add(newAddress.Trim());
                        p.DefaultAddress = newAddress.Trim();
                        AddressEditor.Text = p.DefaultAddress;
                        _profileService.SaveProfile();
                        await DisplayAlertAsync("Address Saved", "New delivery address has been saved and set as default.", "OK");
                    }
                }
                else
                {
                    p.DefaultAddress = action;
                    AddressEditor.Text = action;
                    _profileService.SaveProfile();
                    await DisplayAlertAsync("Address Updated", $"Default address set to:\n{action}", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] AddressShortcut error: {ex.Message}");
            }
        }

        /// <summary>Opens payment methods manager to select or add payment options.</summary>
        private async void OnPaymentShortcutClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);

                var p = _profileService.Profile;
                var paymentOptions = p.SavedPaymentMethods.ToList();
                paymentOptions.Add("➕ Link New Payment Method");

                string action = await DisplayActionSheetAsync(
                    "💳 Saved Payment Methods",
                    "Cancel",
                    null,
                    paymentOptions.ToArray());

                if (string.IsNullOrEmpty(action) || action == "Cancel") return;

                if (action == "➕ Link New Payment Method")
                {
                    string? newMethod = await DisplayPromptAsync(
                        "Add Payment Method",
                        "Enter payment option (e.g. Maya, PayPal, Card):",
                        "Link",
                        "Cancel",
                        placeholder: "📱 Maya (0918-***-9999)");

                    if (!string.IsNullOrWhiteSpace(newMethod))
                    {
                        p.SavedPaymentMethods.Add(newMethod.Trim());
                        p.DefaultPaymentMethod = newMethod.Trim();
                        _profileService.SaveProfile();
                        await DisplayAlertAsync("Payment Method Linked", $"\"{newMethod.Trim()}\" is now linked to your account.", "OK");
                    }
                }
                else
                {
                    p.DefaultPaymentMethod = action;
                    _profileService.SaveProfile();
                    await DisplayAlertAsync("Default Payment", $"Default payment method set to:\n{action}", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] PaymentShortcut error: {ex.Message}");
            }
        }

        /// <summary>Toggles dark mode and dynamically updates application theme.</summary>
        private void OnDarkModeToggled(object? sender, ToggledEventArgs e)
        {
            try
            {
                var p = _profileService.Profile;
                p.IsDarkMode = e.Value;
                _profileService.SaveProfile();

                if (Application.Current != null)
                {
                    Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
                }

                if (ThemeSubtitleLabel != null)
                {
                    ThemeSubtitleLabel.Text = e.Value
                        ? "Ultra-smooth Dark Charcoal theme active"
                        : "Light theme active";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] DarkMode error: {ex.Message}");
            }
        }

        /// <summary>Opens the 24/7 Help Center &amp; Support action sheet.</summary>
        private async void OnHelpCenterTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 40, upMs: 65);

                string action = await DisplayActionSheetAsync(
                    "🎧 FreshMart Help Center & Support",
                    "Close",
                    null,
                    "💬 Chat with Live Support Agent",
                    "📞 Call 24/7 Customer Hotline",
                    "❓ Frequently Asked Questions (FAQ)",
                    "📦 Report Issue with Recent Order");

                if (string.IsNullOrEmpty(action) || action == "Close") return;

                if (action == "💬 Chat with Live Support Agent")
                {
                    await DisplayAlertAsync(
                        "Live Support Connected",
                        "Agent 'Sarah' from FreshMart Support is ready to assist you.\n\n" +
                        "💬 Average response time: < 1 minute.\nYour session ticket: #FM-88492",
                        "Start Chat");
                }
                else if (action == "📞 Call 24/7 Customer Hotline")
                {
                    await DisplayAlertAsync(
                        "Customer Hotline",
                        "Dialing FreshMart VIP Priority Line:\n\n" +
                        "📞 1-800-FRESH-MART (Toll-Free)\nOperating 24 hours a day, 7 days a week.",
                        "Call Now");
                }
                else if (action == "❓ Frequently Asked Questions (FAQ)")
                {
                    await DisplayAlertAsync(
                        "FreshMart FAQs",
                        "• Express Delivery: 20-30 min within Metro areas.\n" +
                        "• Promo Codes: Enter 'SAVE20' or 'FREESHIP' at Cart review.\n" +
                        "• Payment Methods: We accept Cash on Delivery, GCash, and Credit Cards.\n" +
                        "• Order Tracking: View live status in your Profile Order History or Admin.",
                        "Got It");
                }
                else if (action == "📦 Report Issue with Recent Order")
                {
                    var orders = _profileService.GetMyOrders();
                    if (orders.Count > 0)
                    {
                        var latest = orders.First();
                        await DisplayAlertAsync(
                            "Order Support Ticket",
                            $"Ticket created for Order #{latest.Id} ({latest.Status}).\nOur culinary dispatch team will review your order within 10 minutes.",
                            "Submit Ticket");
                    }
                    else
                    {
                        await DisplayAlertAsync("Support", "No recent orders to report. You can reach out directly via Live Chat!", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] HelpCenter error: {ex.Message}");
            }
        }

        /// <summary>Displays logout confirmation dialog with profile switch options.</summary>
        private async void OnLogoutTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 40, upMs: 65);

                string action = await DisplayActionSheetAsync(
                    "🚪 Account & Session",
                    "Cancel",
                    "Log Out Juan Dela Cruz",
                    "👤 Switch to Admin Demo Account",
                    "🔄 Reset Session & Relogin as Guest");

                if (string.IsNullOrEmpty(action) || action == "Cancel") return;

                if (action == "Log Out Juan Dela Cruz" || action == "🔄 Reset Session & Relogin as Guest")
                {
                    bool confirm = await DisplayAlertAsync("Log Out", "Are you sure you want to log out?", "Log Out", "Cancel");
                    if (confirm)
                    {
                        await DisplayAlertAsync("Logged Out", "You have safely signed out. Reconnecting as guest session.", "Continue");
                        PopulateProfile();
                    }
                }
                else if (action == "👤 Switch to Admin Demo Account")
                {
                    var p = _profileService.Profile;
                    p.FullName = "FreshMart Admin";
                    p.Email = "admin@freshmart.delivery";
                    p.ContactNumber = "0999-888-7777";
                    _profileService.SaveProfile();
                    PopulateProfile();
                    await DisplayAlertAsync("Account Switched", "Switched to FreshMart Admin account.\nYou can access management tools via the Admin and Reports tabs.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] Logout error: {ex.Message}");
            }
        }

        /// <summary>Displays an itemized receipt modal when an order is tapped.</summary>
        private async void OnOrderItemTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.96, downMs: 35, upMs: 55);

                Order? order = null;
                if (e.Parameter is Order ord)
                {
                    order = ord;
                }
                else if (sender is BindableObject bo && bo.BindingContext is Order ord2)
                {
                    order = ord2;
                }

                if (order == null) return;

                string itemsList = order.Items != null && order.Items.Count > 0
                    ? string.Join("\n", order.Items.Select(i => $"  • {i.Quantity}x {i.Product?.Name} (${i.Subtotal:F2})"))
                    : "  • Standard Items Bundle";

                await DisplayAlertAsync(
                    $"🧾 Receipt · Order #{order.Id}",
                    $"Customer: {order.CustomerName}\n" +
                    $"Status: {order.Status}\n" +
                    $"Payment: {order.PaymentMethod}\n" +
                    $"Scheduled: {order.ScheduledDeliveryDate:ddd, MMM d yyyy • hh:mm tt}\n\n" +
                    $"Delivering to:\n{order.ShippingAddress}\n\n" +
                    $"Order Items:\n{itemsList}\n\n" +
                    $"Grand Total: ${order.TotalAmount:F2}",
                    "Close Receipt");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] OrderItemTap error: {ex.Message}");
            }
        }

        // ── Save Profile Handler ─────────────────────────────────────────────
        private async void OnSaveProfileClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 45, upMs: 70);

                string name    = NameEntry.Text?.Trim()    ?? string.Empty;
                string email   = EmailEntry.Text?.Trim()   ?? string.Empty;
                string contact = ContactEntry.Text?.Trim() ?? string.Empty;
                string address = AddressEditor.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(name))
                {
                    await DisplayAlertAsync("Missing Info", "Full name cannot be empty.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                {
                    await DisplayAlertAsync("Invalid Email", "Please enter a valid email address.", "OK");
                    return;
                }

                var p = _profileService.Profile;
                p.FullName       = name;
                p.Email          = email;
                p.ContactNumber  = contact;
                p.DefaultAddress = address;
                _profileService.SaveProfile();

                PopulateProfile();
                await DisplayAlertAsync("Profile Saved", "Your profile details have been successfully updated.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] SaveProfile error: {ex.Message}");
                await DisplayAlertAsync("Error", "Could not save profile. Please try again.", "OK");
            }
        }
    }
}
