namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a vendor/grocery store in the Explore Shops view.
    /// </summary>
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; } = 5.0;
        public int DeliveryMinutes { get; set; } = 10;
        public string Category { get; set; } = "Grocery";
        public string RatingStars => "★★★★★";
        public string DeliveryTime => $"{DeliveryMinutes} mins";
    }
}
