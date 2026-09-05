using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a single line item inside a shopping cart.
    /// Implements INotifyPropertyChanged so quantity and subtotal changes
    /// propagate immediately to UI bindings without collection resets.
    /// </summary>
    public class CartItem : INotifyPropertyChanged
    {
        private int _quantity = 1;
        private SubstitutionOption _substitutionPreference = SubstitutionOption.AutomaticReplacement;
        private int? _fallbackProductId;

        /// <summary>The product that was added to the cart.</summary>
        public Product Product { get; set; } = new();

        /// <summary>Number of units of this product in the cart.</summary>
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Subtotal));
                    OnPropertyChanged(nameof(FormattedSubtotal));
                }
            }
        }

        /// <summary>Customer preference for replacements when this item is out of stock.</summary>
        public SubstitutionOption SubstitutionPreference
        {
            get => _substitutionPreference;
            set
            {
                if (_substitutionPreference != value)
                {
                    _substitutionPreference = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SubstitutionPreferenceDisplay));
                    OnPropertyChanged(nameof(SubstitutionBadgeText));
                }
            }
        }

        /// <summary>Optional specific alternative product ID preferred as replacement.</summary>
        public int? FallbackProductId
        {
            get => _fallbackProductId;
            set
            {
                if (_fallbackProductId != value)
                {
                    _fallbackProductId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Human-readable substitution preference display text.</summary>
        public string SubstitutionPreferenceDisplay => SubstitutionPreference.ToDisplayText();

        /// <summary>Short badge text for substitution preference.</summary>
        public string SubstitutionBadgeText => SubstitutionPreference.ToShortBadgeText();

        /// <summary>
        /// Calculated total cost for this line item (Price × Quantity).
        /// </summary>
        public decimal Subtotal => (Product?.Price ?? 0m) * Quantity;

        /// <summary>Formatted subtotal string for display.</summary>
        public string FormattedSubtotal => $"${Subtotal:F2}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
