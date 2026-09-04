using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for OrdersPage — manages order list dynamically, category tab filters
    /// (Ongoing, Past, Cancelled), and search filtering.
    /// </summary>
    public class OrdersViewModel : BaseViewModel
    {
        private string _selectedTab = "Ongoing";
        private string _searchText = string.Empty;
        private List<Order> _currentTabOrders = [];

        public ObservableCollection<Order> Orders { get; } = [];

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    LoadOrdersForTab();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterOrders(_searchText);
                }
            }
        }

        public Order? ActiveOrder => MockDataService.Orders.FirstOrDefault(o => o.Status == "Out for Delivery" || o.Status == "Preparing Order" || o.Status == "Pending");
        public bool HasActiveOrder => ActiveOrder != null;

        public OrdersViewModel()
        {
            Title = "Orders";
            LoadOrdersForTab();
        }

        public void LoadOrdersForTab()
        {
            try
            {
                IsBusy = true;
                _currentTabOrders = MockDataService.GetOrdersByTab(SelectedTab);
                FilterOrders(SearchText);
                OnPropertyChanged(nameof(ActiveOrder));
                OnPropertyChanged(nameof(HasActiveOrder));
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void FilterOrders(string query)
        {
            string clean = (query ?? string.Empty).Trim().ToLowerInvariant();

            Orders.Clear();
            var matches = string.IsNullOrWhiteSpace(clean)
                ? _currentTabOrders
                : _currentTabOrders.Where(o =>
                    o.VendorName.ToLowerInvariant().Contains(clean) ||
                    o.Id.ToString().Contains(clean) ||
                    (o.Line1Preview ?? string.Empty).ToLowerInvariant().Contains(clean));

            foreach (var ord in matches)
            {
                Orders.Add(ord);
            }
        }

        public void SetTab(string tabName)
        {
            SelectedTab = tabName;
        }
    }
}
