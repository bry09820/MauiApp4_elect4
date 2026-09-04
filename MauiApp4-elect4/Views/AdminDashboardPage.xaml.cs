using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    /// <summary>
    /// Admin Dashboard — manage products, view/update orders with status filters,
    /// view inline sales analytics, inspect order receipts, and reset application data.
    /// Hardware-accelerated with GPU micro-interactions and dark design system tokens.
    /// </summary>
    public partial class AdminDashboardPage : ContentPage
    {
        // ── State ────────────────────────────────────────────────────────────
        private List<Product> _products = [];
        private readonly MockDataService _dataService = new();

        // Active filter for the Orders tab
        private string _activeOrderFilter = "All";

        // Design system colors
        private static readonly Color ColActiveTabBg   = Color.FromArgb("#FF6B4A"); // Coral
        private static readonly Color ColActiveTabTxt  = Colors.White;
        private static readonly Color ColInactiveTabBg = Color.FromArgb("#22262B"); // Dark elevated panel
        private static readonly Color ColInactiveTabTxt = Color.FromArgb("#8A94A6"); // Muted platinum

        public AdminDashboardPage()
        {
            InitializeComponent();
        }

        // ── Lifecycle ────────────────────────────────────────────────────────
        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                LoadProducts();
                ApplyOrderFilter(_activeOrderFilter, updateChips: true);
                if (AnalyticsPanel.IsVisible)
                {
                    RefreshAnalyticsKpis();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] OnAppearing error: {ex.Message}");
            }
        }

        // ── Data loading ─────────────────────────────────────────────────────
        private void LoadProducts()
        {
            _products = _dataService.GetProducts();
            AdminProductsView.ItemsSource = null;
            AdminProductsView.ItemsSource = _products;
        }

        /// <summary>
        /// Filters the orders list and optionally refreshes the chip count badges.
        /// </summary>
        private void ApplyOrderFilter(string filter, bool updateChips = false)
        {
            _activeOrderFilter = filter;

            var all = MockDataService.GetOrders();

            if (updateChips) UpdateChipBadges(all);

            var filtered = filter == "All"
                ? all
                : all.Where(o => o.Status.Equals(filter, StringComparison.OrdinalIgnoreCase))
                     .ToList();

            bool hasOrders = filtered.Count > 0;
            NoOrdersLabel.IsVisible = !hasOrders;

            AdminOrdersView.ItemsSource = null;
            AdminOrdersView.ItemsSource = filtered;
        }

        /// <summary>
        /// Updates every chip button's Text to include a live order count badge.
        /// </summary>
        private void UpdateChipBadges(List<Order> all)
        {
            int totalCount       = all.Count;
            int pendingCount     = all.Count(o => o.Status == "Pending");
            int processingCount  = all.Count(o => o.Status == "Processing");
            int outCount         = all.Count(o => o.Status == "Out for Delivery");
            int deliveredCount   = all.Count(o => o.Status == "Delivered");

            FilterAllBtn.Text            = $"All ({totalCount})";
            FilterPendingBtn.Text        = $"🕐 Pending ({pendingCount})";
            FilterProcessingBtn.Text     = $"⚙️ Processing ({processingCount})";
            FilterOutForDeliveryBtn.Text = $"🚚 Out ({outCount})";
            FilterDeliveredBtn.Text      = $"✅ Done ({deliveredCount})";
        }

        private void RefreshAnalyticsKpis()
        {
            var orders     = MockDataService.GetOrders();
            int total      = orders.Count;
            decimal revenue = orders.Sum(o => o.TotalAmount);
            decimal avg    = total > 0 ? revenue / total : 0m;
            int delivered  = orders.Count(o => o.Status == "Delivered");

            QuickRevenueLabel.Text   = $"${revenue:F2}";
            QuickOrdersLabel.Text    = total.ToString();
            QuickAvgLabel.Text       = $"${avg:F2}";
            QuickDeliveredLabel.Text = delivered.ToString();
        }

        private static async Task PressPopAsync(VisualElement el,
                                                double scale  = 0.90,
                                                uint   downMs = 45,
                                                uint   upMs   = 70)
        {
            await el.ScaleToAsync(scale, downMs, Easing.CubicIn);
            await el.ScaleToAsync(1.0,   upMs,   Easing.SpringOut);
        }

        // ── Tab toggle ────────────────────────────────────────────────────────
        private void ShowPanel(string panel)
        {
            ProductsPanel.IsVisible  = panel == "Products";
            OrdersPanel.IsVisible    = panel == "Orders";
            AnalyticsPanel.IsVisible = panel == "Analytics";

            SetTabActive(ProductsTabButton,  panel == "Products");
            SetTabActive(OrdersTabButton,    panel == "Orders");
            SetTabActive(AnalyticsTabButton, panel == "Analytics");
        }

        private static void SetTabActive(Button btn, bool active)
        {
            btn.BackgroundColor = active ? ColActiveTabBg   : ColInactiveTabBg;
            btn.TextColor       = active ? ColActiveTabTxt  : ColInactiveTabTxt;
        }

        // ── Tab click handlers ────────────────────────────────────────────────
        private async void OnProductsTabClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);
                ShowPanel("Products");
                LoadProducts();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Admin] ProductsTab error: {ex.Message}"); }
        }

        private async void OnOrdersTabClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);
                ShowPanel("Orders");
                ApplyOrderFilter(_activeOrderFilter, updateChips: true);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Admin] OrdersTab error: {ex.Message}"); }
        }

        private async void OnAnalyticsTabClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 40, upMs: 65);
                ShowPanel("Analytics");
                RefreshAnalyticsKpis();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Admin] AnalyticsTab error: {ex.Message}"); }
        }

        // ── Filter chips ──────────────────────────────────────────────────────
        private async void OnFilterChipClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not Button chip) return;
                await PressPopAsync(chip, scale: 0.90, downMs: 40, upMs: 65);

                string filter = chip.CommandParameter as string ?? "All";
                ApplyOrderFilter(filter, updateChips: true);
                HighlightActiveChip(filter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] FilterChip error: {ex.Message}");
            }
        }

        private void HighlightActiveChip(string filter)
        {
            Button[] chips =
            [
                FilterAllBtn, FilterPendingBtn, FilterProcessingBtn,
                FilterOutForDeliveryBtn, FilterDeliveredBtn
            ];
            foreach (var c in chips)
            {
                c.BackgroundColor = ColInactiveTabBg;
                c.TextColor       = ColInactiveTabTxt;
            }

            var active = filter switch
            {
                "Pending"          => FilterPendingBtn,
                "Processing"       => FilterProcessingBtn,
                "Out for Delivery" => FilterOutForDeliveryBtn,
                "Delivered"        => FilterDeliveredBtn,
                _                  => FilterAllBtn
            };
            active.BackgroundColor = ColActiveTabBg;
            active.TextColor       = ColActiveTabTxt;
        }

        // ── Analytics shortcut ────────────────────────────────────────────────
        private async void OnOpenReportsClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 45, upMs: 70);
                await Shell.Current.GoToAsync(nameof(ReportsPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] OpenReports error: {ex.Message}");
            }
        }

        // ── Reset handler ─────────────────────────────────────────────────────
        private async void OnResetDataClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 45, upMs: 70);

                bool confirm = await DisplayAlertAsync(
                    "⚠️ Reset Application Data",
                    "This will permanently clear ALL placed orders and empty the shopping cart.\n\n" +
                    "Product catalogue data will be restored to its original state.\n\n" +
                    "Continue?",
                    "Yes, Reset", "Cancel");

                if (!confirm) return;

                MockDataService.ResetData();
                CartService.Instance.ClearCart();

                LoadProducts();
                ApplyOrderFilter("All", updateChips: true);
                HighlightActiveChip("All");

                if (AnalyticsPanel.IsVisible)
                    RefreshAnalyticsKpis();

                await DisplayAlertAsync(
                    "✅ Reset Complete",
                    "All orders and cart data have been cleared.\nThe app is ready for a fresh demo.",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] ResetData error: {ex.Message}");
                await DisplayAlertAsync("Error", "Reset failed. Please try again.", "OK");
            }
        }

        // ── Product handlers ──────────────────────────────────────────────────
        private async void OnAddProductClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 45, upMs: 70);

                string? name = await DisplayPromptAsync(
                    "Add Dish", "Dish name:", "Next", "Cancel",
                    placeholder: "e.g. Truffle Mushroom Pasta");
                if (string.IsNullOrWhiteSpace(name)) return;

                string? category = await DisplayPromptAsync(
                    "Add Dish", "Category (Dairy / Bakery / Fruits / Beverages):",
                    "Next", "Cancel", placeholder: "Bakery");
                if (string.IsNullOrWhiteSpace(category)) return;

                string? priceText = await DisplayPromptAsync(
                    "Add Dish", "Price (e.g. 14.99):",
                    "Next", "Cancel", keyboard: Keyboard.Numeric);
                if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
                {
                    await DisplayAlertAsync("Invalid Price", "Please enter a valid positive price.", "OK");
                    return;
                }

                string? stockText = await DisplayPromptAsync(
                    "Add Dish", "Stock quantity:",
                    "Save", "Cancel", keyboard: Keyboard.Numeric);
                if (!int.TryParse(stockText, out int stock) || stock < 0)
                {
                    await DisplayAlertAsync("Invalid Stock", "Please enter a valid non-negative stock quantity.", "OK");
                    return;
                }

                int newId = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
                _products.Add(new Product
                {
                    Id            = newId,
                    Name          = name.Trim(),
                    Category      = category.Trim(),
                    Price         = price,
                    StockQuantity = stock,
                    ImageUrl      = "dotnet_bot.png"
                });

                AdminProductsView.ItemsSource = null;
                AdminProductsView.ItemsSource = _products;

                await DisplayAlertAsync("Dish Added", $"\"{name}\" has been added to the catalogue.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] AddProduct error: {ex.Message}");
                await DisplayAlertAsync("Error", "Could not add dish. Please try again.", "OK");
            }
        }

        private async void OnEditStockClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not Button btn || btn.CommandParameter is not Product product) return;
                await PressPopAsync(btn, scale: 0.90, downMs: 40, upMs: 65);

                string? input = await DisplayPromptAsync(
                    "Edit Stock",
                    $"New stock quantity for \"{product.Name}\":",
                    "Save", "Cancel",
                    initialValue: product.StockQuantity.ToString(),
                    keyboard: Keyboard.Numeric);

                if (!int.TryParse(input, out int newStock) || newStock < 0)
                {
                    if (input is not null)
                        await DisplayAlertAsync("Invalid Stock", "Please enter a valid non-negative number.", "OK");
                    return;
                }

                product.StockQuantity = newStock;
                AdminProductsView.ItemsSource = null;
                AdminProductsView.ItemsSource = _products;

                await DisplayAlertAsync("Stock Updated",
                    $"\"{product.Name}\" stock updated to {newStock} units.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] EditStock error: {ex.Message}");
            }
        }

        private async void OnDeleteProductClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not Button btn || btn.CommandParameter is not Product product) return;
                await PressPopAsync(btn, scale: 0.90, downMs: 40, upMs: 65);

                bool confirm = await DisplayAlertAsync(
                    "Delete Dish",
                    $"Delete \"{product.Name}\" from the catalogue?",
                    "Delete", "Cancel");

                if (!confirm) return;

                _products.Remove(product);
                AdminProductsView.ItemsSource = null;
                AdminProductsView.ItemsSource = _products;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] DeleteProduct error: {ex.Message}");
            }
        }

        // ── Order status picker ───────────────────────────────────────────────
        private void OnStatusPickerChanged(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not Picker picker) return;
                if (picker.SelectedItem is not string newStatus) return;

                Element? current = picker.Parent;
                while (current is not null)
                {
                    if (current is BindableObject bo && bo.BindingContext is Order order)
                    {
                        if (order.Status != newStatus)
                        {
                            MockDataService.UpdateOrderStatus(order, newStatus);
                            ApplyOrderFilter(_activeOrderFilter, updateChips: true);
                        }
                        return;
                    }
                    current = current.Parent;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] StatusPicker error: {ex.Message}");
            }
        }

        // ── Admin Order Tap Handler ──────────────────────────────────────────
        private async void OnAdminOrderTapped(object? sender, TappedEventArgs e)
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

                string action = await DisplayActionSheetAsync(
                    $"📋 Order #{order.Id} Details",
                    "Close",
                    null,
                    "📦 Update Status to 'Processing'",
                    "🚚 Update Status to 'Out for Delivery'",
                    "✅ Mark as 'Delivered'",
                    "📞 Call Customer (" + order.ContactNumber + ")");

                if (string.IsNullOrEmpty(action) || action == "Close") return;

                if (action.Contains("Processing"))
                {
                    MockDataService.UpdateOrderStatus(order, "Processing");
                    ApplyOrderFilter(_activeOrderFilter, updateChips: true);
                    await DisplayAlertAsync("Status Updated", $"Order #{order.Id} marked as Processing.", "OK");
                }
                else if (action.Contains("Out for Delivery"))
                {
                    MockDataService.UpdateOrderStatus(order, "Out for Delivery");
                    ApplyOrderFilter(_activeOrderFilter, updateChips: true);
                    await DisplayAlertAsync("Status Updated", $"Order #{order.Id} marked as Out for Delivery.", "OK");
                }
                else if (action.Contains("Delivered"))
                {
                    MockDataService.UpdateOrderStatus(order, "Delivered");
                    ApplyOrderFilter(_activeOrderFilter, updateChips: true);
                    await DisplayAlertAsync("Status Updated", $"Order #{order.Id} marked as Delivered.", "OK");
                }
                else if (action.Contains("Call Customer"))
                {
                    await DisplayAlertAsync("Call Customer", $"Contacting {order.CustomerName} at {order.ContactNumber}...", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Admin] OrderTap error: {ex.Message}");
            }
        }
    }
}
