using MauiApp4_elect4.Models;

namespace MauiApp4_elect4.Services
{
    /// <summary>
    /// Singleton service that holds the current customer's profile for the session.
    /// Replace with an authentication / persistence layer in production.
    /// </summary>
    public sealed class UserProfileService
    {
        private static readonly Lazy<UserProfileService> _instance =
            new(() => new UserProfileService(), isThreadSafe: true);

        /// <summary>The one shared instance of the profile for the app lifetime.</summary>
        public static UserProfileService Instance => _instance.Value;

        private UserProfileService() { }

        /// <summary>The active user profile. Always non-null (defaults are pre-populated).</summary>
        public UserProfile Profile { get; } = new UserProfile();

        /// <summary>
        /// Persists any in-place edits made to <see cref="Profile"/>.
        /// In production this would call an API; here it is a no-op because
        /// the property setters on UserProfile already mutate the same object.
        /// </summary>
        public void SaveProfile() { /* no-op — object is shared by reference */ }

        /// <summary>
        /// Returns all orders placed by the current user (matched by CustomerName).
        /// </summary>
        public List<Order> GetMyOrders()
        {
            string name = Profile.FullName;
            return MockDataService.Orders
                .Where(o => o.CustomerName.Equals(name, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(o => o.Id)
                .ToList();
        }
    }
}
