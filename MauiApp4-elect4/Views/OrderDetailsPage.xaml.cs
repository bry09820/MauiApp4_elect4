using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.Views
{
    [QueryProperty(nameof(OrderIdString), "orderId")]
    public partial class OrderDetailsPage : ContentPage
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
                    LoadOrder();
                }
            }
        }

        public OrderDetailsPage()
        {
            InitializeComponent();
            LoadOrder();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadOrder();
        }

        private void LoadOrder()
        {
            try
            {
                _currentOrder = MockDataService.GetOrderById(_orderId) ?? MockDataService.GetLatestOrder();

                OrderIdHeaderLabel.Text = _currentOrder.OrderNumberDisplay;
                VendorNameLabel.Text = _currentOrder.VendorName;
                VendorIconLabel.Text = _currentOrder.VendorIcon;
                OrderStatusLabel.Text = _currentOrder.Status;
                AddressDetailsLabel.Text = _currentOrder.ShippingAddress;

                SummaryTotalTopLabel.Text = _currentOrder.TotalAmount.ToString("C");
                SubtotalLabel.Text = _currentOrder.Subtotal.ToString("C");
                DeliveryFeeLabel.Text = _currentOrder.DeliveryFee.ToString("C");
                TotalAmountLabel.Text = _currentOrder.TotalAmount.ToString("C");

                if (!string.IsNullOrWhiteSpace(_currentOrder.CourierName))
                {
                    CourierNameLabel.Text = _currentOrder.CourierName;
                }

                if (_currentOrder.Items.Count > 0)
                {
                    ItemsDetailsCollectionView.ItemsSource = _currentOrder.Items;
                }
                else
                {
                    ItemsDetailsCollectionView.ItemsSource = new List<CartItem>
                    {
                        new CartItem { Product = new Product { Name = "Fresh Lettuce", Price = 1.99m, ImageUrl = "https://images.unsplash.com/photo-1556801712-76c8eb07bbc9?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 },
                        new CartItem { Product = new Product { Name = "Sourdough Bread", Price = 1.99m, ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 },
                        new CartItem { Product = new Product { Name = "Orange Juice", Price = 1.29m, ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 }
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderDetailsPage] LoadOrder error: {ex.Message}");
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
                await Shell.Current.GoToAsync("..");
            }
            catch
            {
                await Shell.Current.GoToAsync("//ExploreShopsPage");
            }
        }

        private async void OnCallCourierClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.90, downMs: 35, upMs: 55);

                await DisplayAlertAsync(
                    "Connecting Call...",
                    $"Dialing courier {_currentOrder.CourierName} at {_currentOrder.CourierPhone} via secure relay.",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrderDetailsPage] Call error: {ex.Message}");
            }
        }
    }
}
