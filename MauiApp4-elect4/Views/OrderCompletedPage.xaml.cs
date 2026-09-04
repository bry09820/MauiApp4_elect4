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
                await Shell.Current.GoToAsync("//ExploreShopsPage");
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
                await Shell.Current.GoToAsync("//ExploreShopsPage");
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
                await Shell.Current.GoToAsync($"{nameof(TrackOrderPage)}?orderId={_currentOrder.Id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderCompletedPage] TrackOrders error: {ex.Message}");
                await Shell.Current.GoToAsync(nameof(TrackOrderPage));
            }
        }
    }
}
