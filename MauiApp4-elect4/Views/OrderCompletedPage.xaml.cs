using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    [QueryProperty(nameof(OrderIdString), "orderId")]
    public partial class OrderCompletedPage : ContentPage
    {
        private Order _currentOrder = new();
        private int _orderId = 104;

        public string OrderIdString
        {
            get => _orderId.ToString();
            set
            {
                if (int.TryParse(value, out int id))
                {
                    _orderId = id;
                    LoadCompletedOrder();
                }
            }
        }

        public OrderCompletedPage()
        {
            InitializeComponent();
            LoadCompletedOrder();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCompletedOrder();
            CleanupPrecedingNavigationStack();
        }

        protected override bool OnBackButtonPressed()
        {
            // Intercept system/hardware back button to navigate to Home cleanly
            // instead of popping back into the Checkout/Cart stack
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await NavigateAndClearStackAsync("//ExploreShopsPage");
            });
            return true;
        }

        private void LoadCompletedOrder()
        {
            try
            {
                _currentOrder = MockDataService.GetOrderById(_orderId) ?? MockDataService.GetLatestOrder();

                OrderNumberMetaLabel.Text = _currentOrder.OrderNumberDisplay;
                OrderPriceMetaLabel.Text = _currentOrder.TotalAmount.ToString("C");
                CompletedSubtotalLabel.Text = _currentOrder.Subtotal.ToString("C");
                CompletedDeliveryFeeLabel.Text = _currentOrder.DeliveryFee.ToString("C");
                CompletedTotalLabel.Text = _currentOrder.TotalAmount.ToString("C");

                if (_currentOrder.Items.Count > 0)
                {
                    CompletedItemsCollectionView.ItemsSource = _currentOrder.Items;
                }
                else
                {
                    CompletedItemsCollectionView.ItemsSource = new List<CartItem>
                    {
                        new CartItem { Product = new Product { Name = "Fresh Lettuce", Price = 1.99m, ImageUrl = "https://images.unsplash.com/photo-1556801712-76c8eb07bbc9?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 },
                        new CartItem { Product = new Product { Name = "Sourdough Bread", Price = 1.99m, ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 },
                        new CartItem { Product = new Product { Name = "Orange Juice", Price = 1.29m, ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 }
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] LoadCompletedOrder error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans up preceding pages (such as CheckoutPage) from the navigation stack
        /// so pressing back never re-enters checkout.
        /// </summary>
        private void CleanupPrecedingNavigationStack()
        {
            try
            {
                var nav = Navigation;
                if (nav?.NavigationStack != null && nav.NavigationStack.Count > 1)
                {
                    var pagesToRemove = nav.NavigationStack
                        .Where(p => p != this && (p is CheckoutPage || p is CartPage || p is MyCartPage))
                        .ToList();

                    foreach (var page in pagesToRemove)
                    {
                        nav.RemovePage(page);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] CleanupPrecedingNavigationStack error: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the navigation backstack and navigates to the specified root shell route.
        /// </summary>
        private async Task NavigateAndClearStackAsync(string targetRoute)
        {
            try
            {
                // Pop backstack to root so current tab is clean
                if (Navigation?.NavigationStack?.Count > 1)
                {
                    await Navigation.PopToRootAsync(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] PopToRootAsync error: {ex.Message}");
            }

            try
            {
                await Shell.Current.GoToAsync(targetRoute);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] Shell.GoToAsync error: {ex.Message}");
            }
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
                await NavigateAndClearStackAsync("//ExploreShopsPage");
            }
            catch
            {
                await Shell.Current.GoToAsync("//ExploreShopsPage");
            }
        }

        private async void OnContinueShoppingClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.94, downMs: 40);
                await NavigateAndClearStackAsync("//ExploreShopsPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] ContinueShopping error: {ex.Message}");
                await Shell.Current.GoToAsync("//ExploreShopsPage");
            }
        }

        private async void OnTrackOrdersClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.94, downMs: 40);
                await NavigateAndClearStackAsync("//OrdersPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] TrackOrders error: {ex.Message}");
                await Shell.Current.GoToAsync("//OrdersPage");
            }
        }
    }
}
