namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a single line item inside a shopping cart.
    /// </summary>
    public class CartItem
    {
        /// <summary>The product that was added to the cart.</summary>
        public Product Product { get; set; } = new();

        /// <summary>Number of units of this product in the cart.</summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Calculated total cost for this line item (Price × Quantity).
        /// This is a read-only computed property — no backing field required.
        /// </summary>
        public decimal Subtotal => Product.Price * Quantity;
    }
}
