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
        public bool HasPendingSubstitutions => ActiveOrder?.HasPendingSubstitutions == true;

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
                OnPropertyChanged(nameof(HasPendingSubstitutions));
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ApproveSubstitution(PickerSubstitutionRequest req, Order order)
        {
            if (req == null || order == null) return;
            req.Status = SubstitutionStatus.Approved;

            var matchingItem = order.Items.FirstOrDefault(i =>
                i.Product != null &&
                (i.Product.Name.Contains("Sourdough", StringComparison.OrdinalIgnoreCase) ||
                 req.OriginalItemName.Contains(i.Product.Name, StringComparison.OrdinalIgnoreCase)));

            if (matchingItem != null)
            {
                matchingItem.Product.Name = req.ProposedItemName;
                matchingItem.Product.Price = req.ProposedItemPrice;
            }

            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.TotalAmount = Math.Max(0m, order.Subtotal + order.DeliveryFee - order.DiscountAmount);

            LoadOrdersForTab();
        }

        public void DeclineSubstitution(PickerSubstitutionRequest req, Order order)
        {
            if (req == null || order == null) return;
            req.Status = SubstitutionStatus.DeclinedRefunded;

            var matchingItem = order.Items.FirstOrDefault(i =>
                i.Product != null &&
                (i.Product.Name.Contains("Sourdough", StringComparison.OrdinalIgnoreCase) ||
                 req.OriginalItemName.Contains(i.Product.Name, StringComparison.OrdinalIgnoreCase)));

            if (matchingItem != null)
            {
                order.Items.Remove(matchingItem);
            }

            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.TotalAmount = Math.Max(0m, order.Subtotal + order.DeliveryFee - order.DiscountAmount);

            LoadOrdersForTab();
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
