namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents the customer's preferred handling when an ordered grocery item is out of stock.
    /// </summary>
    public enum SubstitutionOption
    {
        /// <summary>Automatically substitute with organic or similar high-quality brand.</summary>
        AutomaticReplacement = 0,

        /// <summary>Do not substitute; immediately refund the item price to the customer.</summary>
        RefundImmediately = 1,

        /// <summary>Send real-time photo and prompt via live picker chat for review.</summary>
        ContactShopper = 2
    }

    /// <summary>
    /// Extension helper methods for user-friendly formatting of substitution options.
    /// </summary>
    public static class SubstitutionOptionExtensions
    {
        public const string AutomaticReplacementText = "Substitute with Organic/Similar Brand";
        public const string RefundImmediatelyText = "Refund Item Immediately";
        public const string ContactShopperText = "Ask via Live Chat";

        public static string ToDisplayText(this SubstitutionOption option) => option switch
        {
            SubstitutionOption.AutomaticReplacement => AutomaticReplacementText,
            SubstitutionOption.RefundImmediately => RefundImmediatelyText,
            SubstitutionOption.ContactShopper => ContactShopperText,
            _ => AutomaticReplacementText
        };

        public static string ToShortBadgeText(this SubstitutionOption option) => option switch
        {
            SubstitutionOption.AutomaticReplacement => "🔄 Auto-Replace",
            SubstitutionOption.RefundImmediately => "💰 Refund if OOS",
            SubstitutionOption.ContactShopper => "💬 Ask via Chat",
            _ => "🔄 Auto-Replace"
        };

        public static SubstitutionOption FromDisplayText(string? text) => text switch
        {
            AutomaticReplacementText => SubstitutionOption.AutomaticReplacement,
            RefundImmediatelyText => SubstitutionOption.RefundImmediately,
            ContactShopperText => SubstitutionOption.ContactShopper,
            _ => SubstitutionOption.AutomaticReplacement
        };

        public static List<string> GetAllDisplayOptions() =>
        [
            AutomaticReplacementText,
            RefundImmediatelyText,
            ContactShopperText
        ];
    }
}
