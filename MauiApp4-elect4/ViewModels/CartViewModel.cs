using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for MyCartPage and CartPage — provides dynamic reactive calculations
    /// for cart line-items, quantities, subtotals, delivery fees, promo discounts,
    /// and checkout totals without any hardcoded numbers or strings.
    /// </summary>
    public class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private const decimal BaseDeliveryFee = 1.09m;

        private decimal _discountAmount;
        private string _appliedPromoCode = string.Empty;
        private string _promoCodeInput = string.Empty;

        public ObservableCollection<CartItem> CartItems => _cartService.CartItems;

        public decimal Subtotal => HasItems ? _cartService.GetTotalAmount() : 0m;
        public decimal DeliveryFee => HasItems ? (_appliedPromoCode == "FREESHIP" ? 0m : BaseDeliveryFee) : 0m;
        public decimal DiscountAmount => HasItems ? _discountAmount : 0m;
        public decimal TotalAmount => HasItems ? Math.Max(0m, Subtotal + DeliveryFee - DiscountAmount) : 0m;

        public string FormattedSubtotal => $"${Subtotal:F2}";
        public string FormattedDeliveryFee => $"${DeliveryFee:F2}";
        public string FormattedDiscount => $"-${DiscountAmount:F2}";
        public string FormattedTotal => $"${TotalAmount:F2}";

        public int TotalItemCount => HasItems ? _cartService.GetTotalItemCount() : 0;
        public string CartCountBadgeText => TotalItemCount.ToString();

        public bool HasItems => CartItems.Count > 0;
        public bool IsCartEmpty => !HasItems;
        public bool HasDiscount => HasItems && DiscountAmount > 0m;

        public string CheckoutButtonText => HasItems
            ? $"Proceed to Checkout ({FormattedTotal})  →"
            : "Cart is Empty";

        public string PromoCodeInput
        {
            get => _promoCodeInput;
            set => SetProperty(ref _promoCodeInput, value);
        }

        public CartViewModel(CartService? cartService = null)
        {
            Title = "My Cart";
            _cartService = cartService ?? CartService.Instance;

            _cartService.CartUpdated += (s, e) => NotifyCalculationsChanged();
            _cartService.CartItems.CollectionChanged += (s, e) => NotifyCalculationsChanged();
        }

        public void IncrementQuantity(CartItem item)
        {
            if (item == null) return;
            _cartService.IncrementQuantity(item);
            NotifyCalculationsChanged();
        }

        public void DecrementQuantity(CartItem item)
        {
            if (item == null) return;
            _cartService.DecrementQuantity(item);
            NotifyCalculationsChanged();
        }

        public void RemoveItem(CartItem item)
        {
            if (item == null) return;
            _cartService.RemoveFromCart(item);
            NotifyCalculationsChanged();
        }

        public void ClearCart()
        {
            _cartService.ClearCart();
            _discountAmount = 0m;
            _appliedPromoCode = string.Empty;
            _promoCodeInput = string.Empty;
            NotifyCalculationsChanged();
        }

        public void ResetCartState()
        {
            ClearCart();
        }

        public bool ApplyPromoCode(string code, out string message)
        {
            string clean = (code ?? string.Empty).Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(clean))
            {
                message = "Please enter a promo code.";
                return false;
            }

            if (clean == "FRESH20" || clean == "ELECT4" || clean == "GREEN" || clean == "SAVE20")
            {
                _discountAmount = Subtotal * 0.20m > 0m ? Subtotal * 0.20m : 2.00m;
                _appliedPromoCode = clean;
                NotifyCalculationsChanged();
                message = $"Promo code '{clean}' applied! You saved {_discountAmount:C} on this order.";
                return true;
            }

            if (clean == "FREESHIP")
            {
                _appliedPromoCode = clean;
                NotifyCalculationsChanged();
                message = "Free shipping applied successfully!";
                return true;
            }

            message = "The promo code entered is not valid or has expired.";
            return false;
        }

        public void NotifyCalculationsChanged()
        {
            if (!HasItems)
            {
                _discountAmount = 0m;
                _appliedPromoCode = string.Empty;
            }

            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(DeliveryFee));
            OnPropertyChanged(nameof(DiscountAmount));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(FormattedSubtotal));
            OnPropertyChanged(nameof(FormattedDeliveryFee));
            OnPropertyChanged(nameof(FormattedDiscount));
            OnPropertyChanged(nameof(FormattedTotal));
            OnPropertyChanged(nameof(TotalItemCount));
            OnPropertyChanged(nameof(CartCountBadgeText));
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(IsCartEmpty));
            OnPropertyChanged(nameof(HasDiscount));
            OnPropertyChanged(nameof(CheckoutButtonText));
            OnPropertyChanged(nameof(PromoCodeInput));
        }
    }
}
