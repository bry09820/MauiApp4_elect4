using MauiApp4_elect4.Models;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    /// <summary>
    /// Explore Shops page — header address bar, promo delivery banner, categories,
    /// top vendors, and popular grocery deals bound dynamically to ExploreShopsViewModel.
    /// </summary>
    public partial class ExploreShopsPage : ContentPage
    {
        public ExploreShopsViewModel ViewModel { get; }

        public ExploreShopsPage()
        {
            InitializeComponent();
            ViewModel = new ExploreShopsViewModel();
            BindingContext = ViewModel;

            TopVendorsCollectionView.ItemsSource = ViewModel.TopVendors;
            PopularDealsCollectionView.ItemsSource = ViewModel.PopularDeals;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadData();
        }

        // ── Shared micro-animation ───────────────────────────────────────────
        private static async Task PressPopAsync(VisualElement el, double scale = 0.92, uint downMs = 45, uint upMs = 65)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0, upMs, Easing.SpringOut);
        }

        // ── Top Header Actions ────────────────────────────────────────────────
        private async void OnFavoritesTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await DisplayAlertAsync("Favorites", "You have saved 4 favorite stores and 12 items.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] Favs error: {ex.Message}");
            }
        }

        private async void OnInfoTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await DisplayAlertAsync("FreshMart Info", "🌱 Fresh Organic Produce & Groceries delivered to your door in 20-30 mins.", "Got It");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] Info error: {ex.Message}");
            }
        }

        private async void OnAddressPinTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                string? result = await DisplayPromptAsync(
                    "Delivery Address",
                    "Enter your delivery address:",
                    initialValue: ViewModel.DeliveryAddress,
                    maxLength: 80,
                    keyboard: Keyboard.Text
                );

                if (!string.IsNullOrWhiteSpace(result))
                {
                    ViewModel.DeliveryAddress = result.Trim();
                    await DisplayAlertAsync("Address Updated", $"Delivery location set to:\n{ViewModel.DeliveryAddress}", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] Address prompt error: {ex.Message}");
            }
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            ViewModel.SearchText = e.NewTextValue ?? string.Empty;
        }

        // ── Category Grid Taps ────────────────────────────────────────────────
        private async void OnCategoryCardTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                string category = e.Parameter as string ?? "All";
                await Shell.Current.GoToAsync($"{nameof(GreenMarketPage)}?category={Uri.EscapeDataString(category)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] Category nav error: {ex.Message}");
            }
        }

        // ── Vendor Cards ──────────────────────────────────────────────────────
        private async void OnVendorCardTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                if (e.Parameter is Vendor vendor)
                {
                    await Shell.Current.GoToAsync($"{nameof(GreenMarketPage)}?vendorName={Uri.EscapeDataString(vendor.Name)}");
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(GreenMarketPage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] Vendor nav error: {ex.Message}");
            }
        }

        private async void OnSeeAllVendorsTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync(nameof(GreenMarketPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] See all vendors error: {ex.Message}");
            }
        }

        private async void OnSeeAllDealsTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await Shell.Current.GoToAsync(nameof(GreenMarketPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] See all deals error: {ex.Message}");
            }
        }

        // ── Product Quick Add ─────────────────────────────────────────────────
        private async void OnAddToCartClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    await PressPopAsync(btn, 0.88, 35, 55);

                    if (btn.CommandParameter is Product product)
                    {
                        ViewModel.AddToCart(product);

                        // Visual feedback
                        string originalText = btn.Text;
                        btn.Text = "✓ Added";
                        btn.BackgroundColor = Color.FromArgb("#15522A");

                        await Task.Delay(750);

                        btn.Text = originalText;
                        btn.BackgroundColor = Color.FromArgb("#1E6B39");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExploreShopsPage] Add to cart error: {ex.Message}");
            }
        }
    }
}
