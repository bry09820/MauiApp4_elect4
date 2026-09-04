namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a placed grocery delivery order.
    /// </summary>
    public class Order
    {
        /// <summary>Unique identifier for the order.</summary>
        public int Id { get; set; } = 104;

        /// <summary>Vendor name.</summary>
        public string VendorName { get; set; } = "GreenMarket";

        /// <summary>Vendor icon or emoji.</summary>
        public string VendorIcon { get; set; } = "🌿";

        /// <summary>Full name of the customer who placed the order.</summary>
        public string CustomerName { get; set; } = "Alex Rivera";

        /// <summary>Contact phone number provided at checkout.</summary>
        public string ContactNumber { get; set; } = "+1 (555) 019-2834";

        /// <summary>Shipping address entered at checkout.</summary>
        public string ShippingAddress { get; set; } = "742 Evergreen Terrace, Springfield, OR 97477";

        /// <summary>Selected delivery method ("Home Delivery", "Pickup").</summary>
        public string DeliveryMethod { get; set; } = "Home Delivery";

        /// <summary>Selected payment method (e.g., "Credit Card", "Digital Wallet", "Cash on Delivery").</summary>
        public string PaymentMethod { get; set; } = "Credit Card";

        /// <summary>Snapshot of cart items included in this order.</summary>
        public List<CartItem> Items { get; set; } = [];

        /// <summary>Subtotal amount before fees and discounts.</summary>
        public decimal Subtotal { get; set; } = 11.99m;

        /// <summary>Delivery fee amount.</summary>
        public decimal DeliveryFee { get; set; } = 1.09m;

        /// <summary>Discount amount applied via promo.</summary>
        public decimal DiscountAmount { get; set; } = 0m;

        /// <summary>Grand total including delivery fee and discounts.</summary>
        public decimal TotalAmount { get; set; } = 13.08m;

        /// <summary>The date and time the customer scheduled the delivery.</summary>
        public DateTime ScheduledDeliveryDate { get; set; } = DateTime.Now.AddMinutes(15);

        /// <summary>Estimated delivery time in minutes.</summary>
        public int EstimatedMinutes { get; set; } = 15;

        /// <summary>Courier driver name.</summary>
        public string CourierName { get; set; } = "Mike Roberts";

        /// <summary>Courier driver phone number.</summary>
        public string CourierPhone { get; set; } = "+1 (555) 839-2041";

        /// <summary>Courier driver photo.</summary>
        public string CourierPhotoUrl { get; set; } = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=300&auto=format&fit=crop&q=80";

        /// <summary>
        /// Current fulfillment status of the order.
        /// Valid values: "Out for Delivery", "Preparing Order", "Delivered", "Cancelled".
        /// </summary>
        public string Status { get; set; } = "Out for Delivery";

        /// <summary>Relative or formatted delivery time subtitle (e.g. "Delivered 1 day ago").</summary>
        public string DeliveryStatusText { get; set; } = "Out for Delivery";

        /// <summary>Tab classification: "Ongoing", "Past", "Cancelled".</summary>
        public string TabCategory { get; set; } = "Ongoing";

        /// <summary>Order number string format e.g. Ord #104.</summary>
        public string OrderNumberDisplay => $"Ord #{Id}";

        /// <summary>Primary line item text for compact list preview.</summary>
        public string Line1Preview { get; set; } = "🥬 Fresh Lettuce, Sourdough Bread";

        /// <summary>Secondary line item text for compact list preview.</summary>
        public string Line2Preview { get; set; } = "🧃 Orange Juice";

        /// <summary>Action button label (e.g. "Track", "View Details").</summary>
        public string ActionButtonText => Status == "Delivered" ? "View Details" : "Track";

        /// <summary>
        /// Human-readable summary of item count for display.
        /// </summary>
        public string ItemSummary =>
            $"{Items.Sum(i => i.Quantity)} item(s) — ${TotalAmount:F2}";
    }
}
