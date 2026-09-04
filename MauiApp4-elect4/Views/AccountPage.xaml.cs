using MauiApp4_elect4.Services;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    public partial class AccountPage : ContentPage
    {
        public AccountViewModel ViewModel { get; }

        public AccountPage()
        {
            InitializeComponent();
            ViewModel = new AccountViewModel();
            BindingContext = ViewModel;
        }

        private static async Task PressPopAsync(VisualElement el, double scale = 0.94, uint downMs = 40, uint upMs = 60)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0, upMs, Easing.SpringOut);
        }

        private async void OnEditProfileTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                string newName = await DisplayPromptAsync(
                    "Edit Profile Name",
                    "Update your display name:",
                    "Save", "Cancel",
                    initialValue: ViewModel.FullName);

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    ViewModel.UpdateName(newName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] EditProfile error: {ex.Message}");
            }
        }

        private async void OnMyOrdersTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync(nameof(OrdersPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] OrdersNav error: {ex.Message}");
            }
        }

        private async void OnDeliveryAddressTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                string action = await DisplayActionSheetAsync(
                    "📍 Select or Add Delivery Address",
                    "Cancel",
                    null,
                    "🎯 Use Current GPS Location",
                    "📍 Lipa City, Batangas, Philippines",
                    "🏢 Batangas City, Batangas, Philippines",
                    "🏠 123 Sampaguita St., Quezon City, Metro Manila",
                    "➕ Enter Custom Address (Nominatim Geocoded)");

                if (string.IsNullOrEmpty(action) || action == "Cancel")
                    return;

                if (action == "🎯 Use Current GPS Location")
                {
                    await ViewModel.DetectAndSetCurrentLocationAsync();
                    await DisplayAlertAsync("GPS Location Updated", $"Current location set to:\n{ViewModel.DefaultAddress}\nCoordinates: {ViewModel.FormattedCoordinates}", "OK");
                    return;
                }

                if (action == "➕ Enter Custom Address (Nominatim Geocoded)")
                {
                    string custom = await DisplayPromptAsync(
                        "Custom Address",
                        "Enter city or street address (e.g., Lipa City, Batangas):",
                        "Geocode & Save",
                        "Cancel",
                        initialValue: ViewModel.DefaultAddress);

                    if (!string.IsNullOrWhiteSpace(custom))
                    {
                        await ViewModel.UpdateAddressAsync(custom);
                        await DisplayAlertAsync("Address Geocoded", $"Location set to:\n{ViewModel.DefaultAddress}\nGPS Coordinates: {ViewModel.FormattedCoordinates}", "OK");
                    }
                    return;
                }

                string cleaned = action.Replace("📍 ", "").Replace("🏢 ", "").Replace("🏠 ", "");
                await ViewModel.UpdateAddressAsync(cleaned);
                await DisplayAlertAsync("Address Updated", $"Default shipping location set to:\n{ViewModel.DefaultAddress}\nGPS Coordinates: {ViewModel.FormattedCoordinates}", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] Address error: {ex.Message}");
            }
        }

        private async void OnPaymentMethodsTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                await DisplayActionSheetAsync(
                    "💳 Saved Payment Methods",
                    "Close",
                    null,
                    "💳 Visa Card ending in •••• 4242 (Default)",
                    "💳 Mastercard ending in •••• 8819",
                    "📱 Digital Wallet Connected",
                    "➕ Link New Card");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] Payment error: {ex.Message}");
            }
        }

        private async void OnManageVendorsTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] AdminNav error: {ex.Message}");
            }
        }

        private async void OnPromotionsTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                await DisplayAlertAsync(
                    "🎁 Available Promotions",
                    "• SAVE20 - 20% off all organic vegetables & fruits\n• FREESHIP - Free express delivery on orders over $15\n• FEAST10 - $10 off orders above $40",
                    "Got It");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] Promo error: {ex.Message}");
            }
        }

        private async void OnSupportHelpTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                await DisplayAlertAsync(
                    "🎧 Customer Support & Live Help",
                    "Need assistance with your delivery?\n\n• Live Chat: Available 24/7 in app\n• Phone: 1-800-FRESH-MART\n• Email: support@greenmarket.app",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] Support error: {ex.Message}");
            }
        }

        private async void OnSettingsTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                await DisplayActionSheetAsync(
                    "⚙️ Application Settings",
                    "Done",
                    null,
                    "🔔 Push Notifications: Enabled",
                    "📍 Location Services: Precise (GPS)",
                    "🛡️ Biometric Login: Face ID Active",
                    "🌐 Language: English (US)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] Settings error: {ex.Message}");
            }
        }

        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                bool confirm = await DisplayAlertAsync(
                    "Log Out",
                    "Are you sure you want to log out of your account?",
                    "Log Out", "Cancel");

                if (confirm)
                {
                    await Shell.Current.GoToAsync("//ExploreShopsPage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountPage] Logout error: {ex.Message}");
            }
        }
    }
}
