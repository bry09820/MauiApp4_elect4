using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;
using MauiApp4_elect4.Services;

namespace MauiApp4_elect4.ViewModels
{
    /// <summary>
    /// ViewModel for CheckoutPage — handles delivery address, shipping and payment selection,
    /// promo discounts, reactive total calculation, and order submission with automatic cart reset.
    /// </summary>
    public class CheckoutViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly UserProfileService _profileService;
        private const decimal BaseDeliveryFee = 1.09m;

        private string _shippingAddress = "742 Evergreen Terrace, Springfield, OR 97477";
        private string _selectedDeliveryMethod = "Home Delivery";
        private string _selectedPaymentMethod = "Credit Card";
        private string _promoCodeInput = string.Empty;
        private string _appliedPromoCode = string.Empty;
        private decimal _discountAmount;
        private string _promoFeedbackText = string.Empty;
        private bool _isPromoFeedbackVisible;
        private bool _isPromoSuccess;

        public ObservableCollection<CartItem> CartItems => _cartService.CartItems;

        public string ShippingAddress
        {
            get => _shippingAddress;
            set => SetProperty(ref _shippingAddress, value);
        }

        public string SelectedDeliveryMethod
        {
            get => _selectedDeliveryMethod;
            set
            {
                if (SetProperty(ref _selectedDeliveryMethod, value))
                {
                    OnPropertyChanged(nameof(IsHomeDelivery));
                    OnPropertyChanged(nameof(IsPickup));
                    NotifyTotalsChanged();
                }
            }
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                if (SetProperty(ref _selectedPaymentMethod, value))
                {
                    OnPropertyChanged(nameof(IsCreditCard));
                    OnPropertyChanged(nameof(IsWallet));
                }
            }
        }

        public string PromoCodeInput
        {
            get => _promoCodeInput;
            set => SetProperty(ref _promoCodeInput, value);
        }

        public string PromoFeedbackText
        {
            get => _promoFeedbackText;
            set => SetProperty(ref _promoFeedbackText, value);
        }

        public bool IsPromoFeedbackVisible
        {
            get => _isPromoFeedbackVisible;
            set => SetProperty(ref _isPromoFeedbackVisible, value);
        }

        public bool IsPromoSuccess
        {
            get => _isPromoSuccess;
            set => SetProperty(ref _isPromoSuccess, value);
        }

        public bool IsHomeDelivery => SelectedDeliveryMethod == "Home Delivery";
        public bool IsPickup => SelectedDeliveryMethod == "Pickup";
        public bool IsCreditCard => SelectedPaymentMethod == "Credit Card";
        public bool IsWallet => SelectedPaymentMethod == "Digital Wallet";

        public decimal Subtotal => _cartService.GetTotalAmount();

        public decimal DeliveryFee => SelectedDeliveryMethod == "Home Delivery"
            ? (_appliedPromoCode == "FREESHIP" ? 0m : BaseDeliveryFee)
            : 0m;

        public decimal DiscountAmount => _discountAmount;

        public decimal TotalAmount => Math.Max(0m, Subtotal + DeliveryFee - DiscountAmount);

        public string FormattedSubtotal => $"${Subtotal:F2}";
        public string FormattedDeliveryFee => $"${DeliveryFee:F2}";
        public string FormattedDiscount => $"-${DiscountAmount:F2}";
        public string FormattedTotal => $"${TotalAmount:F2}";

        public CheckoutViewModel(CartService? cartService = null, UserProfileService? profileService = null)
        {
            Title = "Checkout";
            _cartService = cartService ?? CartService.Instance;
            _profileService = profileService ?? UserProfileService.Instance;

            LoadState();

            _cartService.CartUpdated += (s, e) => NotifyTotalsChanged();
            _cartService.CartItems.CollectionChanged += (s, e) => NotifyTotalsChanged();
        }

        public void LoadState()
        {
            try
            {
                var profile = _profileService.Profile;
                if (!string.IsNullOrWhiteSpace(profile?.DefaultAddress))
                {
                    ShippingAddress = profile.DefaultAddress;
                }

                NotifyTotalsChanged();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CheckoutViewModel] LoadState error: {ex.Message}");
            }
        }

        public bool ApplyPromoCode(string code)
        {
            string clean = (code ?? string.Empty).Trim().ToUpperInvariant();
            _promoCodeInput = clean;

            if (string.IsNullOrWhiteSpace(clean))
            {
                _discountAmount = 0m;
                _appliedPromoCode = string.Empty;
                PromoFeedbackText = "Please enter a promo code.";
                IsPromoSuccess = false;
                IsPromoFeedbackVisible = true;
                NotifyTotalsChanged();
                return false;
            }

            if (clean == "SAVE20" || clean == "FRESH20" || clean == "ELECT4" || clean == "GREEN")
            {
                _discountAmount = Subtotal * 0.20m;
                _appliedPromoCode = clean;
                PromoFeedbackText = $"🎉 '{clean}' Applied: -{_discountAmount:C}";
                IsPromoSuccess = true;
                IsPromoFeedbackVisible = true;
                NotifyTotalsChanged();
                return true;
            }

            if (clean == "FREESHIP")
            {
                _discountAmount = BaseDeliveryFee;
                _appliedPromoCode = clean;
                PromoFeedbackText = "🚚 'FREESHIP' Applied: Free Delivery";
                IsPromoSuccess = true;
                IsPromoFeedbackVisible = true;
                NotifyTotalsChanged();
                return true;
            }

            _discountAmount = 0m;
            _appliedPromoCode = string.Empty;
            PromoFeedbackText = "❌ Invalid promo code.";
            IsPromoSuccess = false;
            IsPromoFeedbackVisible = true;
            NotifyTotalsChanged();
            return false;
        }

        public void SetDeliveryMethod(string method)
        {
            SelectedDeliveryMethod = method;
        }

        public void SetPaymentMethod(string method)
        {
            SelectedPaymentMethod = method;
        }

        public void SetAddress(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                ShippingAddress = address;
            }
        }

        /// <summary>
        /// Confirms the payment, creates and registers the new Order, and automatically resets the shopping cart.
        /// </summary>
        public async Task<Order> PlaceOrderAsync()
        {
            decimal subtotal = Subtotal > 0 ? Subtotal : 11.99m;
            decimal fee = DeliveryFee;
            decimal discount = DiscountAmount;
            decimal total = TotalAmount > 0 ? TotalAmount : 13.08m;

            var profile = _profileService.Profile;

            var order = new Order
            {
                CustomerName = !string.IsNullOrWhiteSpace(profile?.FullName) ? profile.FullName : "Alex Rivera",
                ContactNumber = !string.IsNullOrWhiteSpace(profile?.ContactNumber) ? profile.ContactNumber : "+1 (555) 019-2834",
                ShippingAddress = ShippingAddress,
                DeliveryMethod = SelectedDeliveryMethod,
                PaymentMethod = SelectedPaymentMethod,
                Items = [.. _cartService.CartItems],
                Subtotal = subtotal,
                DeliveryFee = fee,
                DiscountAmount = discount,
                TotalAmount = total,
                ScheduledDeliveryDate = DateTime.Now.AddMinutes(15),
                EstimatedMinutes = 15,
                CourierName = "Mike Roberts",
                CourierPhone = "+1 (555) 839-2041",
                Status = "Out for Delivery"
            };

            // Register order in database / mock service
            MockDataService.AddOrder(order);

            // 1. Automatic Cart Reset on Checkout:
            // Clear cart items and reset total calculations to zero
            _cartService.ClearCart();
            _discountAmount = 0m;
            _appliedPromoCode = string.Empty;
            _promoCodeInput = string.Empty;
            IsPromoFeedbackVisible = false;

            NotifyTotalsChanged();

            await Task.Yield();
            return order;
        }

        public void NotifyTotalsChanged()
        {
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(DeliveryFee));
            OnPropertyChanged(nameof(DiscountAmount));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(FormattedSubtotal));
            OnPropertyChanged(nameof(FormattedDeliveryFee));
            OnPropertyChanged(nameof(FormattedDiscount));
            OnPropertyChanged(nameof(FormattedTotal));
        }
    }
}
