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
