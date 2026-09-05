using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    [QueryProperty(nameof(OrderIdString), "orderId")]
    public partial class TrackOrderPage : ContentPage
    {
        public TrackOrderViewModel ViewModel { get; }
        private int _orderId = 104;

        public string OrderIdString
        {
            get => _orderId.ToString();
            set
            {
                if (int.TryParse(value, out int id))
                {
                    _orderId = id;
                    _ = LoadOrderAsync();
                }
            }
        }

        public TrackOrderPage()
        {
            InitializeComponent();
            ViewModel = new TrackOrderViewModel();
            BindingContext = ViewModel;

            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.CurrentOrder) ||
                    e.PropertyName == nameof(ViewModel.HasSubstitutions))
                {
                    if (ViewModel.CurrentOrder != null)
                    {
                        OrderTotalLabel.Text = ViewModel.CurrentOrder.TotalAmount.ToString("C");
                        OrderItemsCollectionView.ItemsSource = null;
                        OrderItemsCollectionView.ItemsSource = ViewModel.CurrentOrder.Items;
                    }
                }
            };

            _ = LoadOrderAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadOrderAsync();
        }

        private async Task LoadOrderAsync()
        {
            try
            {
                await ViewModel.InitializeAsync(_orderId);

                if (ViewModel.CurrentOrder != null)
                {
                    OrderIdHeaderLabel.Text = ViewModel.CurrentOrder.OrderNumberDisplay;
                    OrderTotalLabel.Text = ViewModel.CurrentOrder.TotalAmount.ToString("C");

                    if (!string.IsNullOrWhiteSpace(ViewModel.CurrentOrder.CourierName))
                    {
                        CourierNameLabel.Text = ViewModel.CurrentOrder.CourierName;
                    }

                    if (ViewModel.CurrentOrder.Items != null && ViewModel.CurrentOrder.Items.Count > 0)
                    {
                        OrderItemsCollectionView.ItemsSource = ViewModel.CurrentOrder.Items;
                    }
                    else
                    {
                        // Fallback to sample items from mock
                        OrderItemsCollectionView.ItemsSource = new List<CartItem>
                        {
                            new CartItem { Product = new Product { Name = "Fresh Lettuce", Price = 1.99m, ImageUrl = "https://images.unsplash.com/photo-1556801712-76c8eb07bbc9?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 },
                            new CartItem { Product = new Product { Name = "Sourdough Bread", Price = 1.99m, ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 },
                            new CartItem { Product = new Product { Name = "Orange Juice", Price = 1.29m, ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80" }, Quantity = 1 }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderPage] LoadOrder error: {ex.Message}");
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
                await Shell.Current.GoToAsync("//OrdersPage");
            }
        }

        private async void OnCallCourierClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.90, downMs: 35, upMs: 55);

                string courierName = ViewModel.CurrentOrder?.CourierName ?? "Mike Roberts";
                string courierPhone = ViewModel.CurrentOrder?.CourierPhone ?? "+1 (555) 234-5678";

                await DisplayAlertAsync(
                    "Connecting Call...",
                    $"Dialing courier {courierName} at {courierPhone} via secure relay.",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderPage] Call error: {ex.Message}");
            }
        }

        private async void OnApproveSubstitutionClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 35, upMs: 55);

                // Extract the specific substitution request bound to this button's item
                PickerSubstitutionRequest? req = null;
                if (sender is Button btn && btn.CommandParameter is PickerSubstitutionRequest r)
                    req = r;

                ViewModel.ApproveSubstitution(req);
                RefreshOrderSummaryUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderPage] Approve error: {ex.Message}");
            }
        }

        private async void OnDeclineSubstitutionClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.92, downMs: 35, upMs: 55);

                // Extract the specific substitution request bound to this button's item
                PickerSubstitutionRequest? req = null;
                if (sender is Button btn && btn.CommandParameter is PickerSubstitutionRequest r)
                    req = r;

                ViewModel.DeclineSubstitution(req);
                RefreshOrderSummaryUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderPage] Decline error: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes the order summary CollectionView and total label after substitution state changes.
        /// </summary>
        private void RefreshOrderSummaryUI()
        {
            if (ViewModel.CurrentOrder != null)
            {
                OrderTotalLabel.Text = ViewModel.CurrentOrder.TotalAmount.ToString("C");

                // Swap ItemsSource to force CollectionView to re-evaluate bindings
                var items = ViewModel.CurrentOrder.Items;
                OrderItemsCollectionView.ItemsSource = null;
                OrderItemsCollectionView.ItemsSource = items;
            }
        }

        private async void OnViewOrderCompletedClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.95, downMs: 50);

                int orderId = ViewModel.CurrentOrder?.Id ?? _orderId;
                await Shell.Current.GoToAsync($"{nameof(OrderCompletedPage)}?orderId={orderId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderPage] CompletionNav error: {ex.Message}");
                await Shell.Current.GoToAsync(nameof(OrderCompletedPage));
            }
        }
    }

    /// <summary>
    /// Custom GraphicsView Drawable that renders an animated dashed delivery route on the map.
    /// </summary>
    public class DeliveryRouteDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            // Draw dashed green path from vehicle (start) to destination (end)
            var path = new PathF();
            path.MoveTo(60, 95);
            path.QuadTo(140, 120, 200, 70);
            path.QuadTo(250, 30, dirtyRect.Width - 70, 50);

            canvas.StrokeColor = Color.FromArgb("#1E6B39");
            canvas.StrokeSize = 3.5f;
            canvas.StrokeDashPattern = new float[] { 6, 5 };
            canvas.DrawPath(path);

            canvas.RestoreState();
        }
    }
}
