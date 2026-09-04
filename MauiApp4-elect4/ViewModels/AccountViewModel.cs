using System.Windows.Input;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for AccountPage / ProfilePage — binds user profile, saved addresses,
    /// GPS coordinates, payment methods, and Nominatim geocoding dynamically.
    /// </summary>
    public class AccountViewModel : BaseViewModel
    {
        private readonly UserProfileService _profileService;
        private readonly LocationService _locationService;
        private string _locationStatusMessage = string.Empty;
        private bool _isGeocoding;

        public UserProfile Profile => _profileService.Profile;

        public string FullName
        {
            get => Profile.FullName;
            set
            {
                if (Profile.FullName != value)
                {
                    Profile.FullName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Profile));
                }
            }
        }

        public string Email
        {
            get => Profile.Email;
            set
            {
                if (Profile.Email != value)
                {
                    Profile.Email = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Profile));
                }
            }
        }

        public string ContactNumber
        {
            get => Profile.ContactNumber;
            set
            {
                if (Profile.ContactNumber != value)
                {
                    Profile.ContactNumber = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Profile));
                }
            }
        }

        public string DefaultAddress
        {
            get => Profile.DefaultAddress;
            set
            {
                if (Profile.DefaultAddress != value)
                {
                    Profile.DefaultAddress = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Profile));
                }
            }
        }

        public double Latitude
        {
            get => Profile.Latitude;
            set
            {
                if (Math.Abs(Profile.Latitude - value) > 0.000001)
                {
                    Profile.Latitude = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedCoordinates));
                }
            }
        }

        public double Longitude
        {
            get => Profile.Longitude;
            set
            {
                if (Math.Abs(Profile.Longitude - value) > 0.000001)
                {
                    Profile.Longitude = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedCoordinates));
                }
            }
        }

        public string FormattedCoordinates => Profile.FormattedCoordinates;

        public string LocationStatusMessage
        {
            get => _locationStatusMessage;
            set => SetProperty(ref _locationStatusMessage, value);
        }

        public bool IsGeocoding
        {
            get => _isGeocoding;
            set => SetProperty(ref _isGeocoding, value);
        }

        public ICommand UseCurrentLocationCommand { get; }
        public ICommand UpdateAddressCommand { get; }

        public AccountViewModel(UserProfileService? profileService = null, LocationService? locationService = null)
        {
            Title = "Account";
            _profileService = profileService ?? UserProfileService.Instance;
            _locationService = locationService ?? LocationService.Instance;

            UseCurrentLocationCommand = new Command(async () => await DetectAndSetCurrentLocationAsync());
            UpdateAddressCommand = new Command<string>(async (addr) => await UpdateAddressAsync(addr));

            Profile.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Profile));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(ContactNumber));
                OnPropertyChanged(nameof(DefaultAddress));
                OnPropertyChanged(nameof(Latitude));
                OnPropertyChanged(nameof(Longitude));
                OnPropertyChanged(nameof(FormattedCoordinates));
            };
        }

        public void UpdateName(string newName)
        {
            if (!string.IsNullOrWhiteSpace(newName))
            {
                FullName = newName.Trim();
            }
        }

        /// <summary>
        /// Updates the address and queries Nominatim to get real GPS Latitude and Longitude.
        /// </summary>
        public async Task<bool> UpdateAddressAsync(string newAddress)
        {
            if (string.IsNullOrWhiteSpace(newAddress)) return false;

            try
            {
                IsGeocoding = true;
                LocationStatusMessage = "Geocoding via Nominatim...";

                DefaultAddress = newAddress.Trim();

                // Call Nominatim Geocoding API
                var results = await _locationService.SearchAddressAsync(DefaultAddress);
                if (results != null && results.Count > 0)
                {
                    var first = results[0];
                    Latitude = first.Latitude;
                    Longitude = first.Longitude;
                    LocationStatusMessage = $"Coordinates: {Latitude:F4}, {Longitude:F4}";
                    return true;
                }
                else
                {
                    LocationStatusMessage = "Address updated (approx. coordinates).";
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountViewModel] Geocode error: {ex.Message}");
                LocationStatusMessage = "Geocoding failed, standard coordinates set.";
                return false;
            }
            finally
            {
                IsGeocoding = false;
            }
        }

        /// <summary>
        /// Detects current GPS device location using Microsoft.Maui.Devices.Sensors.Geolocation
        /// and reverse-geocodes with Nominatim.
        /// </summary>
        public async Task<bool> DetectAndSetCurrentLocationAsync()
        {
            try
            {
                IsGeocoding = true;
                LocationStatusMessage = "Acquiring GPS fix...";

                var loc = await _locationService.GetCurrentDeviceLocationAsync();
                if (loc != null)
                {
                    Latitude = loc.Latitude;
                    Longitude = loc.Longitude;

                    if (!string.IsNullOrWhiteSpace(loc.DisplayName))
                    {
                        DefaultAddress = loc.DisplayName;
                    }
                    else if (!string.IsNullOrWhiteSpace(loc.Street))
                    {
                        DefaultAddress = $"{loc.Street}, {loc.City}, {loc.Country}";
                    }
                    else
                    {
                        DefaultAddress = $"GPS: {loc.Latitude:F5}, {loc.Longitude:F5}";
                    }

                    LocationStatusMessage = $"GPS Acquired: {loc.Latitude:F4}, {loc.Longitude:F4}";
                    return true;
                }
                else
                {
                    LocationStatusMessage = "Unable to acquire GPS signal. Check permissions.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AccountViewModel] GPS detection error: {ex.Message}");
                LocationStatusMessage = $"GPS error: {ex.Message}";
                return false;
            }
            finally
            {
                IsGeocoding = false;
            }
        }
    }
}
