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

        /// <summary>Category the product belongs to (e.g., Dairy, Bakery).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Unit price of the product in the local currency.</summary>
        public decimal Price { get; set; }

        /// <summary>URL or local resource path for the product image.</summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>Number of units currently available in stock.</summary>
        public int StockQuantity { get; set; }
    }
}
