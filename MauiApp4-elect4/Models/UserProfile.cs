using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Holds the customer's account details, preferences, addresses, and payment methods.
    /// Implements INotifyPropertyChanged for real-time UI binding updates.
    /// </summary>
    public class UserProfile : INotifyPropertyChanged
    {
        private string _fullName = "Juan Dela Cruz";
        private string _email = "juan.delacruz@email.com";
        private string _contactNumber = "0917-123-4567";
        private string _defaultAddress = "Lipa City, Batangas, Philippines";
        private double _latitude = 13.9419;
        private double _longitude = 121.1644;
        private string _defaultPaymentMethod = "💵 Cash on Delivery";
        private bool _isDarkMode = false;

        /// <summary>Customer's display / full name.</summary>
        public string FullName
        {
            get => _fullName;
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Initials));
                }
            }
        }

        /// <summary>Account e-mail address.</summary>
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Primary contact phone number.</summary>
        public string ContactNumber
        {
            get => _contactNumber;
            set
            {
                if (_contactNumber != value)
                {
                    _contactNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Default shipping address pre-filled at checkout.</summary>
        public string DefaultAddress
        {
            get => _defaultAddress;
            set
            {
                if (_defaultAddress != value)
                {
                    _defaultAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Geocoded GPS Latitude of the default delivery address.</summary>
        public double Latitude
        {
            get => _latitude;
            set
            {
                if (Math.Abs(_latitude - value) > 0.000001)
                {
                    _latitude = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedCoordinates));
                }
            }
        }

        /// <summary>Geocoded GPS Longitude of the default delivery address.</summary>
        public double Longitude
        {
            get => _longitude;
            set
            {
                if (Math.Abs(_longitude - value) > 0.000001)
                {
                    _longitude = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedCoordinates));
                }
            }
        }

        /// <summary>Formatted GPS Coordinates for UI display.</summary>
        public string FormattedCoordinates => $"{Latitude:F4}, {Longitude:F4}";

        /// <summary>List of user's saved addresses.</summary>
        public List<string> SavedAddresses { get; set; } =
        [
            "Lipa City, Batangas, Philippines",
            "Batangas City, Batangas, Philippines",
            "123 Sampaguita St., Barangay Uno, Quezon City, Metro Manila",
            "Unit 802 Grand Tower, BGC, Taguig City, Metro Manila"
        ];

        /// <summary>List of user's saved payment methods.</summary>
        public List<string> SavedPaymentMethods { get; set; } =
        [
            "💵 Cash on Delivery",
            "📱 GCash (0917-***-4567)",
            "💳 Visa Card (ending in 4242)"
        ];

        /// <summary>Selected default payment method.</summary>
        public string DefaultPaymentMethod
        {
            get => _defaultPaymentMethod;
            set
            {
                if (_defaultPaymentMethod != value)
                {
                    _defaultPaymentMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Theme preference state.</summary>
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Computed initials for fallback avatars.</summary>
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName)) return "U";
                var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1) return parts[0][0].ToString().ToUpperInvariant();
                return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
