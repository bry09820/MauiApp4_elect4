using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for GreenMarketPage — manages categorized produce, bestsellers,
    /// dynamic tab switching, and product adding.
    /// </summary>
    public class GreenMarketViewModel : BaseViewModel
    {
        private readonly MockDataService _dataService;
        private readonly CartService _cartService;

        private string _storeName = "GreenMarket";
        private string _selectedCategory = "Featured";
        private int _cartItemCount;

        public ObservableCollection<Product> FreshVegetables { get; } = [];
        public ObservableCollection<Product> Bestsellers { get; } = [];

        public string StoreName
        {
            get => _storeName;
            set => SetProperty(ref _storeName, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    OnCategoryChanged(_selectedCategory);
                }
            }
        }

        public int CartItemCount
        {
            get => _cartItemCount;
            set => SetProperty(ref _cartItemCount, value);
        }

        public GreenMarketViewModel(MockDataService? dataService = null, CartService? cartService = null)
        {
            Title = "GreenMarket";
            _dataService = dataService ?? new MockDataService();
            _cartService = cartService ?? CartService.Instance;

            _cartService.CartUpdated += (s, e) => UpdateCartCount();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                IsBusy = true;
                FreshVegetables.Clear();
                foreach (var item in _dataService.GetFreshVegetables())
                {
                    FreshVegetables.Add(item);
                }

                RefreshBestsellers();
                UpdateCartCount();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void SetCategory(string category)
        {
            SelectedCategory = category;
        }

        private void OnCategoryChanged(string category)
        {
            RefreshBestsellers();
        }

        private void RefreshBestsellers()
        {
            Bestsellers.Clear();

            List<Product> items;
            if (SelectedCategory.Equals("Groceries", StringComparison.OrdinalIgnoreCase))
            {
                items = _dataService.GetFilteredProducts("Dairy", null);
            }
            else if (SelectedCategory.Equals("Beverages", StringComparison.OrdinalIgnoreCase))
            {
                items = _dataService.GetFilteredProducts("Beverages", null);
            }
            else if (SelectedCategory.Equals("Bakery", StringComparison.OrdinalIgnoreCase))
            {
                items = _dataService.GetFilteredProducts("Bakery", null);
            }
            else
            {
                items = _dataService.GetBestsellers();
            }

            foreach (var item in items)
            {
                Bestsellers.Add(item);
            }
        }

        public void AddToCart(Product product)
        {
            if (product == null) return;
            _cartService.AddToCart(product);
            UpdateCartCount();
        }

        private void UpdateCartCount()
        {
            CartItemCount = _cartService.GetTotalItemCount();
        }
    }
}
