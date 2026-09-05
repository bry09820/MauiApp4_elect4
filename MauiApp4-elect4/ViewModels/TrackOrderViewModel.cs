using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for Order Tracking with Leaflet.js map display, Nominatim geocoding, 
    /// and OpenRouteService / OSRM route calculations.
    /// </summary>
    public class TrackOrderViewModel : BaseViewModel
    {
        private readonly LocationService _locationService;
        private readonly UserProfileService _profileService;
        private Order _currentOrder = new();
        private int _orderId = 104;

        private GeoLocation _storeLocation = new()
        {
            Latitude = 13.9550,
            Longitude = 121.1550,
            DisplayName = "GreenMarket Organic Hub, Lipa City, Batangas",
            Street = "Ayala Highway",
            City = "Lipa City",
            State = "Batangas",
            Country = "Philippines"
        };

        private GeoLocation _destinationLocation = new()
        {
            Latitude = 13.9419,
            Longitude = 121.1644,
            DisplayName = "Lipa City, Batangas, Philippines",
            Street = "B. Morada Ave",
            City = "Lipa City",
            State = "Batangas",
            Country = "Philippines"
        };

        private GeoLocation _courierLocation = new()
        {
            Latitude = 13.9485,
            Longitude = 121.1597,
            DisplayName = "Courier En Route"
        };

        private string _distanceText = "2.1 km";
        private string _etaText = "~12 Min";
        private string _shippingAddress = "Lipa City, Batangas, Philippines";
        private string _routingStatus = "OpenRouteService · 2.1 km";
        private string _searchAddressQuery = string.Empty;
        private bool _isCalculatingRoute;
        private int _simulationStepIndex;
        private IDispatcherTimer? _simulationTimer;

        public Order CurrentOrder
        {
            get => _currentOrder;
            set => SetProperty(ref _currentOrder, value);
        }

        public string DistanceText
        {
            get => _distanceText;
            set => SetProperty(ref _distanceText, value);
        }

        public string EtaText
        {
            get => _etaText;
            set => SetProperty(ref _etaText, value);
        }

        public string ShippingAddress
        {
            get => _shippingAddress;
            set => SetProperty(ref _shippingAddress, value);
        }

        public string RoutingStatus
        {
            get => _routingStatus;
            set => SetProperty(ref _routingStatus, value);
        }

        public string SearchAddressQuery
        {
            get => _searchAddressQuery;
            set => SetProperty(ref _searchAddressQuery, value);
        }

        public bool IsCalculatingRoute
        {
            get => _isCalculatingRoute;
            set => SetProperty(ref _isCalculatingRoute, value);
        }

        private bool _isPickerChatExpanded = true;
        public bool IsPickerChatExpanded
        {
            get => _isPickerChatExpanded;
            set => SetProperty(ref _isPickerChatExpanded, value);
        }

        public ObservableCollection<PickerSubstitutionRequest> SubstitutionRequests { get; } = [];
        public bool HasPendingSubstitutions => SubstitutionRequests.Any(s => s.IsPending);
        public bool HasSubstitutions => SubstitutionRequests.Count > 0;

        private bool _isPendingSubstitution = true;
        public bool IsPendingSubstitution
        {
            get => _isPendingSubstitution;
            set => SetProperty(ref _isPendingSubstitution, value);
        }

        private bool _isApproved;
        public bool IsApproved
        {
            get => _isApproved;
            set => SetProperty(ref _isApproved, value);
        }

        private bool _isRefunded;
        public bool IsRefunded
        {
            get => _isRefunded;
            set => SetProperty(ref _isRefunded, value);
        }

        public ObservableCollection<MapMarker> MapMarkers { get; } = [];
        public ObservableCollection<GeoPoint> RouteCoordinates { get; } = [];

        public ICommand RefreshRouteCommand { get; }
        public ICommand SearchAddressCommand { get; }
        public ICommand ToggleSimulationCommand { get; }
        public ICommand UseCurrentLocationCommand { get; }
        public ICommand ApproveSubstitutionCommand { get; }
        public ICommand DeclineSubstitutionCommand { get; }
        public ICommand TogglePickerChatCommand { get; }

        public TrackOrderViewModel(LocationService? locationService = null, UserProfileService? profileService = null)
        {
            _locationService = locationService ?? LocationService.Instance;
            _profileService = profileService ?? UserProfileService.Instance;
            Title = "Track Order";

            RefreshRouteCommand = new Command(async () => await LoadRouteAndMapAsync());
            SearchAddressCommand = new Command<string>(async (query) => await SearchAndSetAddressAsync(query));
            ToggleSimulationCommand = new Command(ToggleSimulation);
            UseCurrentLocationCommand = new Command(async () => await UseDeviceLocationAsync());

            ApproveSubstitutionCommand = new Command<PickerSubstitutionRequest>(ApproveSubstitution);
            DeclineSubstitutionCommand = new Command<PickerSubstitutionRequest>(DeclineSubstitution);
            TogglePickerChatCommand = new Command(() => IsPickerChatExpanded = !IsPickerChatExpanded);
        }

        public void ApproveSubstitution(PickerSubstitutionRequest? req = null)
        {
            var target = req ?? SubstitutionRequests.FirstOrDefault();
            if (target != null)
            {
                target.Status = SubstitutionStatus.Approved;
            }

            IsPendingSubstitution = false;
            IsApproved = true;
            IsRefunded = false;

            // Apply replacement update to CurrentOrder items
            var matchingItem = CurrentOrder.Items.FirstOrDefault(i =>
                i.Product != null &&
                (i.Product.Name.Contains("Sourdough", StringComparison.OrdinalIgnoreCase) ||
                 (target != null && target.OriginalItemName.Contains(i.Product.Name, StringComparison.OrdinalIgnoreCase))));

            if (matchingItem != null && target != null)
            {
                matchingItem.Product.Name = target.ProposedItemName;
                matchingItem.Product.Price = target.ProposedItemPrice;
            }

            // Recalculate totals
            CurrentOrder.Subtotal = CurrentOrder.Items.Sum(i => i.Subtotal);
            CurrentOrder.TotalAmount = Math.Max(0m, CurrentOrder.Subtotal + CurrentOrder.DeliveryFee - CurrentOrder.DiscountAmount);

            OnPropertyChanged(nameof(CurrentOrder));
            OnPropertyChanged(nameof(HasPendingSubstitutions));
            OnPropertyChanged(nameof(HasSubstitutions));
            OnPropertyChanged(nameof(IsPendingSubstitution));
            OnPropertyChanged(nameof(IsApproved));
            OnPropertyChanged(nameof(IsRefunded));
        }

        public void DeclineSubstitution(PickerSubstitutionRequest? req = null)
        {
            var target = req ?? SubstitutionRequests.FirstOrDefault();
            if (target != null)
            {
                target.Status = SubstitutionStatus.DeclinedRefunded;
            }

            IsPendingSubstitution = false;
            IsApproved = false;
            IsRefunded = true;

            // Remove or mark item as refunded in CurrentOrder
            var matchingItem = CurrentOrder.Items.FirstOrDefault(i =>
                i.Product != null &&
                (i.Product.Name.Contains("Sourdough", StringComparison.OrdinalIgnoreCase) ||
                 (target != null && target.OriginalItemName.Contains(i.Product.Name, StringComparison.OrdinalIgnoreCase))));

            if (matchingItem != null)
            {
                CurrentOrder.Items.Remove(matchingItem);
            }

            // Recalculate totals with refund applied
            CurrentOrder.Subtotal = CurrentOrder.Items.Sum(i => i.Subtotal);
            CurrentOrder.TotalAmount = Math.Max(0m, CurrentOrder.Subtotal + CurrentOrder.DeliveryFee - CurrentOrder.DiscountAmount);

            OnPropertyChanged(nameof(CurrentOrder));
            OnPropertyChanged(nameof(HasPendingSubstitutions));
            OnPropertyChanged(nameof(HasSubstitutions));
            OnPropertyChanged(nameof(IsPendingSubstitution));
            OnPropertyChanged(nameof(IsApproved));
            OnPropertyChanged(nameof(IsRefunded));
        }

        public async Task InitializeAsync(int orderId)
        {
            _orderId = orderId;
            CurrentOrder = MockDataService.GetOrderById(orderId) ?? MockDataService.GetLatestOrder();

            SubstitutionRequests.Clear();
            if (CurrentOrder.SubstitutionRequests.Count > 0)
            {
                foreach (var sub in CurrentOrder.SubstitutionRequests)
                {
                    SubstitutionRequests.Add(sub);
                }
            }
            else
            {
                // Provide default demonstration substitution request for order tracking
                var defaultSub = new PickerSubstitutionRequest
                {
                    OrderId = orderId,
                    OriginalItemName = "Sourdough Bread (1.0g)",
                    OriginalItemPrice = 1.99m,
                    ProposedItemName = "Artisan Organic Multigrain Loaf (1.0g)",
                    ProposedItemPrice = 2.49m,
                    PickerName = "Elena Ramos (Store Shopper)",
                    PickerMessage = "Bakery shelf is out of regular Sourdough Bread. I found this freshly baked Organic Multigrain Loaf in Aisle 4 as an organic substitute!",
                    AislePhotoUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=600&auto=format&fit=crop&q=80",
                    Status = SubstitutionStatus.PendingApproval
                };
                CurrentOrder.SubstitutionRequests.Add(defaultSub);
                SubstitutionRequests.Add(defaultSub);
            }

            if (SubstitutionRequests.Count > 0)
            {
                var first = SubstitutionRequests[0];
                IsPendingSubstitution = first.IsPending;
                IsApproved = first.IsApproved;
                IsRefunded = first.IsDeclined;
            }
            else
            {
                IsPendingSubstitution = true;
                IsApproved = false;
                IsRefunded = false;
            }

            OnPropertyChanged(nameof(IsPendingSubstitution));
            OnPropertyChanged(nameof(IsApproved));
            OnPropertyChanged(nameof(IsRefunded));
            OnPropertyChanged(nameof(HasPendingSubstitutions));
            OnPropertyChanged(nameof(HasSubstitutions));

            var profile = _profileService.Profile;

            if (!string.IsNullOrWhiteSpace(CurrentOrder.ShippingAddress))
            {
                ShippingAddress = CurrentOrder.ShippingAddress;
            }
            else if (!string.IsNullOrWhiteSpace(profile?.DefaultAddress))
            {
                ShippingAddress = profile.DefaultAddress;
                CurrentOrder.ShippingAddress = ShippingAddress;
            }

            await LoadRouteAndMapAsync();
        }

        /// <summary>
        /// Geocodes the delivery address and calculates the live OpenRouteService / OSRM driving route.
        /// </summary>
        public async Task LoadRouteAndMapAsync()
        {
            if (IsCalculatingRoute) return;

            try
            {
                IsCalculatingRoute = true;
                RoutingStatus = "Calculating Route (OpenRouteService / OSRM)...";

                var profile = _profileService.Profile;

                // 1. Determine destination coordinates from profile or Nominatim
                if (profile != null && !string.IsNullOrWhiteSpace(profile.DefaultAddress) &&
                    string.Equals(ShippingAddress.Trim(), profile.DefaultAddress.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    profile.Latitude != 0 && profile.Longitude != 0)
                {
                    _destinationLocation = new GeoLocation
                    {
                        Latitude = profile.Latitude,
                        Longitude = profile.Longitude,
                        DisplayName = profile.DefaultAddress
                    };
                }
                else
                {
                    // Geocode via Nominatim API
                    var geocodedResults = await _locationService.SearchAddressAsync(ShippingAddress);
                    if (geocodedResults != null && geocodedResults.Count > 0)
                    {
                        _destinationLocation = geocodedResults[0];
                    }
                    else if (profile != null && profile.Latitude != 0 && profile.Longitude != 0)
                    {
                        _destinationLocation = new GeoLocation
                        {
                            Latitude = profile.Latitude,
                            Longitude = profile.Longitude,
                            DisplayName = ShippingAddress
                        };
                    }
                    else
                    {
                        _destinationLocation = LocationService.DefaultCustomerLocation;
                    }
                }

                // 2. Set Vendor / Store Location near destination (~1.5 km away)
                _storeLocation = new GeoLocation
                {
                    Latitude = _destinationLocation.Latitude + 0.0130,
                    Longitude = _destinationLocation.Longitude - 0.0095,
                    DisplayName = $"{CurrentOrder.VendorName} Organic Hub"
                };

                // Place courier halfway between store and destination
                _courierLocation = new GeoLocation
                {
                    Latitude = (_storeLocation.Latitude + _destinationLocation.Latitude) / 2.0,
                    Longitude = (_storeLocation.Longitude + _destinationLocation.Longitude) / 2.0,
                    DisplayName = $"{CurrentOrder.CourierName} (Delivery Van)"
                };

                // 3. Calculate Directions Route using OpenRouteService / OSRM
                var routeResult = await _locationService.CalculateRouteAsync(
                    _storeLocation.Latitude, _storeLocation.Longitude,
                    _destinationLocation.Latitude, _destinationLocation.Longitude);

                if (routeResult.IsSuccess && routeResult.Coordinates.Count > 0)
                {
                    DistanceText = routeResult.DistanceDisplay;
                    EtaText = routeResult.DurationDisplay;
                    RoutingStatus = $"{routeResult.Provider} · {DistanceText}";

                    RouteCoordinates.Clear();
                    foreach (var pt in routeResult.Coordinates)
                    {
                        RouteCoordinates.Add(pt);
                    }
                }
                else
                {
                    DistanceText = "2.1 km";
                    EtaText = "~12 Min";
                    RoutingStatus = "Standard GPS Route · 2.1 km";
                }

                // 4. Update Interactive Leaflet Map Markers
                MapMarkers.Clear();
                MapMarkers.Add(MapMarker.CreateStore(
                    _storeLocation.Latitude,
                    _storeLocation.Longitude,
                    $"{CurrentOrder.VendorName} Hub",
                    "Order Packed & Dispatched"));

                MapMarkers.Add(MapMarker.CreateCourier(
                    _courierLocation.Latitude,
                    _courierLocation.Longitude,
                    CurrentOrder.CourierName,
                    $"ETA: {EtaText} ({DistanceText})"));

                MapMarkers.Add(MapMarker.CreateDestination(
                    _destinationLocation.Latitude,
                    _destinationLocation.Longitude,
                    "Delivery Address",
                    ShippingAddress));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderViewModel] Route calculation error: {ex.Message}");
                RoutingStatus = "Active Delivery Route";
            }
            finally
            {
                IsCalculatingRoute = false;
            }
        }

        /// <summary>
        /// Geocodes a new address search query and updates the delivery destination.
        /// </summary>
        public async Task SearchAndSetAddressAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            ShippingAddress = query.Trim();
            if (CurrentOrder != null)
            {
                CurrentOrder.ShippingAddress = ShippingAddress;
            }
            await LoadRouteAndMapAsync();
        }

        /// <summary>
        /// Obtains device location and recalculates route.
        /// </summary>
        public async Task UseDeviceLocationAsync()
        {
            try
            {
                RoutingStatus = "Locating via GPS...";
                var loc = await _locationService.GetCurrentDeviceLocationAsync();
                if (loc != null)
                {
                    ShippingAddress = !string.IsNullOrWhiteSpace(loc.DisplayName) ? loc.DisplayName : $"{loc.Latitude:F4}, {loc.Longitude:F4}";
                    if (CurrentOrder != null)
                    {
                        CurrentOrder.ShippingAddress = ShippingAddress;
                    }

                    // Also save to user profile
                    var profile = _profileService.Profile;
                    if (profile != null)
                    {
                        profile.DefaultAddress = ShippingAddress;
                        profile.Latitude = loc.Latitude;
                        profile.Longitude = loc.Longitude;
                    }

                    await LoadRouteAndMapAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackOrderViewModel] GPS error: {ex.Message}");
            }
        }

        /// <summary>
        /// Animates live courier delivery truck moving along the route coordinates.
        /// </summary>
        public void ToggleSimulation()
        {
            if (_simulationTimer?.IsRunning == true)
            {
                _simulationTimer.Stop();
                return;
            }

            if (RouteCoordinates.Count < 2) return;

            _simulationStepIndex = 0;
            _simulationTimer = Application.Current?.Dispatcher.CreateTimer();
            if (_simulationTimer != null)
            {
                _simulationTimer.Interval = TimeSpan.FromSeconds(1.2);
                _simulationTimer.Tick += (s, e) =>
                {
                    if (_simulationStepIndex < RouteCoordinates.Count)
                    {
                        var pt = RouteCoordinates[_simulationStepIndex];
                        _courierLocation.Latitude = pt.Latitude;
                        _courierLocation.Longitude = pt.Longitude;

                        var courierMarker = MapMarkers.FirstOrDefault(m => m.MarkerType == "Courier");
                        if (courierMarker != null)
                        {
                            courierMarker.Latitude = pt.Latitude;
                            courierMarker.Longitude = pt.Longitude;
                        }

                        _simulationStepIndex++;
                    }
                    else
                    {
                        _simulationTimer.Stop();
                    }
                };
                _simulationTimer.Start();
            }
        }
    }
}
