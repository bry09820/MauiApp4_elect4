namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Holds the customer's account details, preferences, addresses, and payment methods.
    /// </summary>
    public class UserProfile
    {
        /// <summary>Customer's display / full name.</summary>
        public string FullName { get; set; } = "Juan Dela Cruz";

        /// <summary>Account e-mail address.</summary>
        public string Email { get; set; } = "juan.delacruz@email.com";

        /// <summary>Primary contact phone number.</summary>
        public string ContactNumber { get; set; } = "0917-123-4567";

        /// <summary>Default shipping address pre-filled at checkout.</summary>
        public string DefaultAddress { get; set; } =
            "123 Sampaguita St., Barangay Uno, Quezon City, Metro Manila";

        /// <summary>List of user's saved addresses.</summary>
        public List<string> SavedAddresses { get; set; } =
        [
            "123 Sampaguita St., Barangay Uno, Quezon City, Metro Manila",
            "Unit 802 Grand Tower, BGC, Taguig City, Metro Manila",
            "45 Emerald Avenue, Ortigas Center, Pasig City"
        ];

        /// <summary>List of user's saved payment methods.</summary>
        public List<string> SavedPaymentMethods { get; set; } =
        [
            "💵 Cash on Delivery",
            "📱 GCash (0917-***-4567)",
            "💳 Visa Card (ending in 4242)"
        ];

        /// <summary>Selected default payment method.</summary>
        public string DefaultPaymentMethod { get; set; } = "💵 Cash on Delivery";

        /// <summary>Theme preference state.</summary>
        public bool IsDarkMode { get; set; } = true;
    }
}
