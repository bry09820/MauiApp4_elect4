using MauiApp4_elect4.Models;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    public partial class OrdersPage : ContentPage
    {
        public OrdersViewModel ViewModel { get; }

        public OrdersPage()
        {
            InitializeComponent();
            ViewModel = new OrdersViewModel();
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadOrdersForTab();
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

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            ViewModel.SearchText = e.NewTextValue ?? string.Empty;
        }

        private void OnTabOngoingClicked(object? sender, EventArgs e)
        {
            SwitchTab("Ongoing", TabOngoing, TabPast, TabCancelled);
        }

        private void OnTabPastClicked(object? sender, EventArgs e)
        {
            SwitchTab("Past", TabPast, TabOngoing, TabCancelled);
        }

        private void OnTabCancelledClicked(object? sender, EventArgs e)
        {
            SwitchTab("Cancelled", TabCancelled, TabOngoing, TabPast);
        }

        private void SwitchTab(string tabName, Button activeBtn, Button inactive1, Button inactive2)
        {
            ViewModel.SetTab(tabName);

            activeBtn.BackgroundColor = Color.FromArgb("#1E6B39");
            activeBtn.TextColor = Colors.White;
            activeBtn.FontAttributes = FontAttributes.Bold;

            inactive1.BackgroundColor = Colors.Transparent;
            inactive1.TextColor = Color.FromArgb("#718096");
            inactive1.FontAttributes = FontAttributes.None;

            inactive2.BackgroundColor = Colors.Transparent;
            inactive2.TextColor = Color.FromArgb("#718096");
            inactive2.FontAttributes = FontAttributes.None;
        }

        private async void OnActiveOrderBannerClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el, scale: 0.90, downMs: 35, upMs: 55);

                if (ViewModel.ActiveOrder != null)
                {
                    await Shell.Current.GoToAsync($"{nameof(TrackOrderPage)}?orderId={ViewModel.ActiveOrder.Id}");
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(TrackOrderPage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrdersPage] ActiveOrderBanner error: {ex.Message}");
            }
        }

        private async void OnOrderActionClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is Order order)
                {
                    await PressPopAsync(btn, scale: 0.90, downMs: 35, upMs: 55);

                    if (order.Status == "Out for Delivery" || order.Status == "Preparing Order" || order.Status == "Pending")
                    {
                        await Shell.Current.GoToAsync($"{nameof(TrackOrderPage)}?orderId={order.Id}");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync($"{nameof(OrderDetailsPage)}?orderId={order.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OrdersPage] OrderAction error: {ex.Message}");
            }
        }
    }
}
