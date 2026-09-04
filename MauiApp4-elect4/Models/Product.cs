namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a grocery product available in the store.
    /// </summary>
    public class Product
    {
        /// <summary>Unique identifier for the product.</summary>
        public int Id { get; set; }

        /// <summary>Display name of the product.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Category the product belongs to (e.g., Dairy, Bakery, Fruits, Vegetables, Snacks).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Unit price of the product in the local currency.</summary>
        public decimal Price { get; set; }

        /// <summary>Optional original price for sale/deal comparison.</summary>
        public decimal? OriginalPrice { get; set; }

        /// <summary>URL or local resource path for the product image.</summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>Number of units currently available in stock.</summary>
        public int StockQuantity { get; set; }

        /// <summary>Weight or package size description (e.g., "1.0kg", "500g", "1 L").</summary>
        public string Weight { get; set; } = "1.0kg";

        /// <summary>Product rating score (e.g. 4.9, 5.0).</summary>
        public double Rating { get; set; } = 4.9;

        /// <summary>Promotional badge text (e.g., "+09", "Fresh", "Bestseller").</summary>
        public string BadgeText { get; set; } = string.Empty;

        /// <summary>Short description / subtitle.</summary>
        public string Subtitle { get; set; } = "Fresh & Organic";

        /// <summary>Associated vendor or store name.</summary>
        public string VendorName { get; set; } = "GreenMarket";

        /// <summary>Formatted price string for display.</summary>
        public string FormattedPrice => $"${Price:F2}";
    }
}
