namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a placed grocery delivery order.
    /// </summary>
    public class Order
    {
        /// <summary>Unique identifier for the order.</summary>
        public int Id { get; set; }

        /// <summary>Full name of the customer who placed the order.</summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>Contact phone number provided at checkout.</summary>
        public string ContactNumber { get; set; } = string.Empty;

        /// <summary>Shipping address entered at checkout.</summary>
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>Selected payment method (e.g., "GCash", "Cash on Delivery", "Credit Card").</summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>Snapshot of cart items included in this order.</summary>
        public List<CartItem> Items { get; set; } = [];

        /// <summary>Grand total including delivery fee.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>The date and time the customer scheduled the delivery.</summary>
        public DateTime ScheduledDeliveryDate { get; set; }

        /// <summary>
        /// Current fulfillment status of the order.
        /// Valid values: "Pending", "Processing", "Out for Delivery", "Delivered".
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Human-readable summary of item count for display in the Admin Dashboard.
        /// </summary>
        public string ItemSummary =>
            $"{Items.Sum(i => i.Quantity)} item(s) — ${TotalAmount:F2}";
    }
}
