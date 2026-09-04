using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;
using MauiApp4_elect4.Views;

namespace MauiApp4_elect4
{
    /// <summary>
    /// Explore Shops page — header address bar, promo delivery banner, categories,
    /// top vendors, and popular grocery deals.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private readonly MockDataService _dataService = new();
        private readonly CartService _cartService = CartService.Instance;

        private List<Vendor> _allVendors = [];
        private List<Product> _allDeals = [];

        public MainPage()
        {
            InitializeComponent();
            LoadData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _allVendors = _dataService.GetTopVendors();
                _allDeals = _dataService.GetPopularDeals();

                TopVendorsCollectionView.ItemsSource = _allVendors;
                PopularDealsCollectionView.ItemsSource = _allDeals;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] LoadData error: {ex.Message}");
            }
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
                await DisplayAlertAsync("Favorites", "You have 4 favorite stores and 12 saved items.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Favorites error: {ex.Message}");
            }
        }

        private async void OnInfoTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await DisplayAlertAsync(
                    "Store Info & Express Delivery",
                    "🌿 FreshMart Green Delivery Network\n• 15-30 min express dispatch\n• 100% organic farm certified\n• Free delivery on orders above $25",
                    "Got It");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Info error: {ex.Message}");
            }
        }

        private async void OnAddressPinTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                string action = await DisplayActionSheetAsync(
                    "📍 Select Delivery Address",
                    "Cancel",
                    null,
                    "📍 742 Evergreen Terrace, Springfield",
                    "🏢 Office: 100 Market St, Suite 400",
                    "🏠 Home: 25 Green Valley Road",
                    "🛰️ Use Current GPS Location");

                if (!string.IsNullOrEmpty(action) && action != "Cancel")
                {
                    SearchAddressEntry.Text = action.Replace("📍 ", "").Replace("🏢 ", "").Replace("🏠 ", "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Pin error: {ex.Message}");
            }
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            try
            {
                string query = e.NewTextValue?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(query))
                {
                    PopularDealsCollectionView.ItemsSource = _allDeals;
                }
                else
                {
                    PopularDealsCollectionView.ItemsSource = _allDeals
                        .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                    p.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Search error: {ex.Message}");
            }
        }

        // ── Category Card Navigation ──────────────────────────────────────────
        private async void OnCategoryCardTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);

                string category = e.Parameter?.ToString() ?? "All";
                await Shell.Current.GoToAsync($"{nameof(GreenMarketPage)}?category={Uri.EscapeDataString(category)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Category error: {ex.Message}");
            }
        }

        // ── Vendor Card Navigation ────────────────────────────────────────────
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
                System.Diagnostics.Debug.WriteLine($"[MainPage] Vendor click error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"[MainPage] SeeAllVendors error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"[MainPage] SeeAllDeals error: {ex.Message}");
            }
        }

        // ── Add to Cart ───────────────────────────────────────────────────────
        private async void OnAddToCartClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is Product product)
                {
                    await PressPopAsync(btn, scale: 0.85, downMs: 35, upMs: 55);

                    _cartService.AddToCart(product);

                    // Quick visual feedback
                    btn.Text = "✓ Added";
                    btn.BackgroundColor = Color.FromArgb("#2B7A4B");
                    await Task.Delay(800);
                    btn.Text = "+ Add";
                    btn.BackgroundColor = Color.FromArgb("#1E6B39");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] AddToCart error: {ex.Message}");
            }
        }
    }
}
