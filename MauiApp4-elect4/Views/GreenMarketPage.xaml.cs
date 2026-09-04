using MauiApp4_elect4.Models;
using MauiApp4_elect4.ViewModels;

namespace MauiApp4_elect4.Views
{
    [QueryProperty(nameof(VendorName), "vendorName")]
    [QueryProperty(nameof(Category), "category")]
    public partial class GreenMarketPage : ContentPage
    {
        public GreenMarketViewModel ViewModel { get; }

        public string VendorName
        {
            get => ViewModel.StoreName;
            set
            {
                string unescaped = Uri.UnescapeDataString(value ?? "GreenMarket");
                ViewModel.StoreName = unescaped;
                if (StoreTitleLabel != null) StoreTitleLabel.Text = unescaped;
            }
        }

        public string Category
        {
            get => ViewModel.SelectedCategory;
            set
            {
                string unescaped = Uri.UnescapeDataString(value ?? "Featured");
                ViewModel.SelectedCategory = unescaped;
                SetActiveTab(unescaped);
            }
        }

        public GreenMarketPage()
        {
            InitializeComponent();
            ViewModel = new GreenMarketViewModel();
            BindingContext = ViewModel;

            VegetablesCollectionView.ItemsSource = ViewModel.FreshVegetables;
            BestsellersCollectionView.ItemsSource = ViewModel.Bestsellers;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadData();
            if (StoreTitleLabel != null) StoreTitleLabel.Text = ViewModel.StoreName;
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

        private async void OnStoreBookmarkTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await DisplayAlertAsync("Store Saved", $"{ViewModel.StoreName} has been added to your favorite stores list.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GreenMarketPage] Bookmark error: {ex.Message}");
            }
        }

        private void OnTabClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    string tabName = btn.Text;
                    SetActiveTab(tabName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GreenMarketPage] Tab error: {ex.Message}");
            }
        }

        private void SetActiveTab(string tabName)
        {
            if (TabFeatured == null || TabGroceries == null || TabBeverages == null || TabBakery == null) return;

            var tabs = new[] { TabFeatured, TabGroceries, TabBeverages, TabBakery };
            foreach (var t in tabs)
            {
                bool isActive = t.Text.Equals(tabName, StringComparison.OrdinalIgnoreCase);
                t.TextColor = isActive ? Colors.White : Color.FromArgb("#B0D5BC");
                t.FontAttributes = isActive ? FontAttributes.Bold : FontAttributes.None;
            }

            ViewModel.SetCategory(tabName);
        }

        private async void OnSeeAllVegTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await DisplayAlertAsync("Fresh Vegetables", "Showing all 24 fresh organic vegetables in stock.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GreenMarketPage] SeeAllVeg error: {ex.Message}");
            }
        }

        private async void OnSeeAllBestsellersTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (sender is VisualElement el) await PressPopAsync(el);
                await DisplayAlertAsync("Bestsellers", "Showing all 18 top rated supermarket bestsellers.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GreenMarketPage] SeeAllBestsellers error: {ex.Message}");
            }
        }

        private async void OnAddToCartClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is Product product)
                {
                    await PressPopAsync(btn, scale: 0.85, downMs: 35, upMs: 55);

                    ViewModel.AddToCart(product);

                    btn.Text = "✓ Added";
                    btn.BackgroundColor = Color.FromArgb("#2B7A4B");
                    await Task.Delay(800);
                    btn.Text = "+ Add";
                    btn.BackgroundColor = Color.FromArgb("#1E6B39");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GreenMarketPage] AddToCart error: {ex.Message}");
            }
        }
    }
}
