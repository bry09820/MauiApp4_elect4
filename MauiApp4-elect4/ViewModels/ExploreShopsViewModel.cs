using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for ExploreShopsPage — binds top vendors, deals catalogue,
    /// dynamic search filtering, category selection, and quick-add to cart.
    /// </summary>
    public class ExploreShopsViewModel : BaseViewModel
    {
        private readonly MockDataService _dataService;
        private readonly CartService _cartService;

        private string _searchText = string.Empty;
        private string _deliveryAddress = "742 Evergreen Terrace, Springfield, OR";
        private int _cartItemCount;

        public ObservableCollection<Vendor> TopVendors { get; } = [];
        public ObservableCollection<Product> PopularDeals { get; } = [];

        private List<Vendor> _allVendors = [];
        private List<Product> _allDeals = [];

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterItems(_searchText);
                }
            }
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value);
        }

        public int CartItemCount
        {
            get => _cartItemCount;
            set => SetProperty(ref _cartItemCount, value);
        }

        public ExploreShopsViewModel(MockDataService? dataService = null, CartService? cartService = null)
        {
            Title = "Explore Shops";
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
                _allVendors = _dataService.GetTopVendors();
                _allDeals = _dataService.GetPopularDeals();

                TopVendors.Clear();
                foreach (var v in _allVendors) TopVendors.Add(v);

                PopularDeals.Clear();
                foreach (var d in _allDeals) PopularDeals.Add(d);

                UpdateCartCount();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void FilterItems(string query)
        {
            string clean = query.Trim().ToLowerInvariant();

            TopVendors.Clear();
            var matchedVendors = string.IsNullOrWhiteSpace(clean)
                ? _allVendors
                : _allVendors.Where(v => v.Name.ToLowerInvariant().Contains(clean) || v.Category.ToLowerInvariant().Contains(clean));
            foreach (var v in matchedVendors) TopVendors.Add(v);

            PopularDeals.Clear();
            var matchedDeals = string.IsNullOrWhiteSpace(clean)
                ? _allDeals
                : _allDeals.Where(d => d.Name.ToLowerInvariant().Contains(clean) || d.Category.ToLowerInvariant().Contains(clean));
            foreach (var d in matchedDeals) PopularDeals.Add(d);
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
