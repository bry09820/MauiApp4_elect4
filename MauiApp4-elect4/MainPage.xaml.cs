using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;
using MauiApp4_elect4.Views;

namespace MauiApp4_elect4
{
    /// <summary>
    /// Product catalogue page — search, category filtering, sort options, and add-to-cart.
    /// Hardware-accelerated with GPU ScaleToAsync() micro-interactions and async data filtering.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        // ── Services ─────────────────────────────────────────────────────────
        private readonly MockDataService    _dataService    = new();
        private readonly CartService        _cartService    = CartService.Instance;
        private readonly UserProfileService _profileService = UserProfileService.Instance;

        // ── Category collection for dynamic data binding ──────────────────────
        private readonly ObservableCollection<CategoryItem> _categories =
        [
            new CategoryItem { Name = "All", Icon = "🔥", IsSelected = true },
            new CategoryItem { Name = "Dairy", Icon = "🥛", IsSelected = false },
            new CategoryItem { Name = "Bakery", Icon = "🥖", IsSelected = false },
            new CategoryItem { Name = "Fruits", Icon = "🍎", IsSelected = false },
            new CategoryItem { Name = "Beverages", Icon = "🧃", IsSelected = false }
        ];

        // ── Filter state ─────────────────────────────────────────────────────
        private string _selectedCategory = "All";
        private string _searchText       = string.Empty;
        private string _activeSortOption = "Default";

        // ── Constructor ──────────────────────────────────────────────────────
        public MainPage()
        {
            InitializeComponent();
            CategoryCollectionView.ItemsSource = _categories;
            _cartService.CartItems.CollectionChanged += OnCartCollectionChanged;
        }

        private void OnCartCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(RefreshCartBadge);
        }

        // ── Lifecycle ────────────────────────────────────────────────────────
        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Refresh product list and badge every time the page appears
                RefreshProductsAsync();
                RefreshCartBadge();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] OnAppearing error: {ex.Message}");
            }
        }

        // ── Shared animation helper ───────────────────────────────────────────

        private static async Task PressPopAsync(VisualElement el,
                                                double scale  = 0.90,
                                                uint   downMs = 45,
                                                uint   upMs   = 70)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0,   upMs,   Easing.SpringOut);
        }

        // ── Product loading & filtering ───────────────────────────────────────

        /// <summary>Queries the service asynchronously with current filter/sort state.</summary>
        private async void RefreshProductsAsync()
        {
            try
            {
                string cat = _selectedCategory;
                string search = _searchText;
                string sort = _activeSortOption;

                // Offload filtering & sorting to worker thread for 60-90 FPS UI fluidity
                var products = await Task.Run(() =>
                {
                    var list = _dataService.GetFilteredProducts(cat, search);
                    return sort switch
                    {
                        "PriceLowHigh" => list.OrderBy(p => p.Price).ToList(),
                        "PriceHighLow" => list.OrderByDescending(p => p.Price).ToList(),
                        "NameAZ"       => list.OrderBy(p => p.Name).ToList(),
                        _              => list
                    };
                });

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProductsCollectionView.ItemsSource = products;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] RefreshProducts error: {ex.Message}");
            }
        }

        // ── Cart badge ───────────────────────────────────────────────────────
        private void RefreshCartBadge()
        {
            try
            {
                int count = _cartService.GetTotalItemCount();
                CartButton.Text = count > 0
                    ? $"🛒  {count}"
                    : "🛒  0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] RefreshCartBadge error: {ex.Message}");
            }
        }

        // ── Interactive Header Elements ───────────────────────────────────────

        private async void OnLocationTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 40, upMs: 65);

                var p = _profileService.Profile;
                var addressOptions = p.SavedAddresses.ToList();
                addressOptions.Add("📍 Use Current GPS Location");

                string action = await DisplayActionSheetAsync(
                    "📍 Select Delivery Location",
                    "Cancel",
                    null,
                    addressOptions.ToArray());

                if (string.IsNullOrEmpty(action) || action == "Cancel") return;

                if (action == "📍 Use Current GPS Location")
                {
                    CurrentLocationLabel.Text = "Current GPS ▾";
                    await DisplayAlertAsync("Location Set", "Delivering to your current GPS coordinates (Fast dispatch active).", "OK");
                }
                else
                {
                    p.DefaultAddress = action;
                    CurrentLocationLabel.Text = "Saved Address ▾";
                    await DisplayAlertAsync("Location Set", $"Delivery location updated to:\n{action}", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] LocationTap error: {ex.Message}");
            }
        }

        private async void OnNotificationBellTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (NotificationBellBorder != null)
                    await PressPopAsync(NotificationBellBorder, scale: 0.88, downMs: 45, upMs: 70);

                var recentOrders = _profileService.GetMyOrders();
                if (recentOrders.Count > 0)
                {
                    var latest = recentOrders.First();
                    await DisplayAlertAsync(
                        "🔔 Order Status Update",
                        $"Order #{latest.Id} is currently '{latest.Status}'!\n" +
                        $"Delivery scheduled for {latest.ScheduledDeliveryDate:ddd, hh:mm tt}.\n\n" +
                        "🔥 Promo: Use 'SAVE20' for 20% off your next order!",
                        "OK");
                }
                else
                {
                    await DisplayAlertAsync(
                        "🔔 FreshMart Notifications",
                        "No active deliveries at the moment.\n\n" +
                        "🎉 Exclusive Offer: Use promo code 'FREESHIP' for free delivery on your first order!",
                        "Got It");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Notification error: {ex.Message}");
            }
        }

        private async void OnFilterIconTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (FilterOptionsBorder != null)
                    await PressPopAsync(FilterOptionsBorder, scale: 0.88, downMs: 45, upMs: 70);

                string action = await DisplayActionSheetAsync(
                    "⊞ Sort Dishes & Groceries",
                    "Cancel",
                    null,
                    "🌟 Featured / Recommended",
                    "💵 Price: Low to High",
                    "💎 Price: High to Low",
                    "🔤 Name: A to Z");

                if (string.IsNullOrEmpty(action) || action == "Cancel") return;

                _activeSortOption = action switch
                {
                    "💵 Price: Low to High" => "PriceLowHigh",
                    "💎 Price: High to Low" => "PriceHighLow",
                    "🔤 Name: A to Z"        => "NameAZ",
                    _                       => "Default"
                };

                RefreshProductsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] FilterIcon error: {ex.Message}");
            }
        }

        // ── Event handlers ───────────────────────────────────────────────────

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            try
            {
                _searchText = e.NewTextValue ?? string.Empty;
                RefreshProductsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Search error: {ex.Message}");
            }
        }

        private async void OnCategoryPillClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is CategoryItem item)
                {
                    await PressPopAsync(btn, scale: 0.88, downMs: 40, upMs: 65);

                    _selectedCategory = item.Name;
                    foreach (var c in _categories)
                    {
                        c.IsSelected = (c.Name == item.Name);
                    }

                    RefreshProductsAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] CategoryPill error: {ex.Message}");
            }
        }

        private async void OnAddToCartClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is Product product)
                {
                    await PressPopAsync(btn, scale: 0.85, downMs: 40, upMs: 65);

                    _cartService.AddToCart(product);
                    RefreshCartBadge();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] AddToCart error: {ex.Message}");
            }
        }

        private async void OnCartButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el)
                    await PressPopAsync(el, scale: 0.92, downMs: 45, upMs: 65);

                await Shell.Current.GoToAsync(nameof(CartPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] CartNav error: {ex.Message}");
            }
        }

        private async void OnClearSearchClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el)
                    await PressPopAsync(el, scale: 0.92, downMs: 45, upMs: 65);

                _searchText       = string.Empty;
                _selectedCategory = "All";
                _activeSortOption = "Default";
                ProductSearchBar.Text = string.Empty;

                foreach (var c in _categories)
                {
                    c.IsSelected = (c.Name == "All");
                }

                RefreshProductsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] ClearSearch error: {ex.Message}");
            }
        }
    }
}
