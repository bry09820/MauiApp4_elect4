using MauiApp4_elect4.Views;

namespace MauiApp4_elect4
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // ── Push/modal routes & aliases ──────────────────────────────────
            Routing.RegisterRoute(nameof(ExploreShopsPage),   typeof(ExploreShopsPage));
            Routing.RegisterRoute(nameof(MyCartPage),          typeof(MyCartPage));
            Routing.RegisterRoute(nameof(GreenMarketPage),     typeof(GreenMarketPage));
            Routing.RegisterRoute(nameof(CartPage),            typeof(CartPage));
            Routing.RegisterRoute(nameof(CheckoutPage),        typeof(CheckoutPage));
            Routing.RegisterRoute(nameof(TrackOrderPage),      typeof(TrackOrderPage));
            Routing.RegisterRoute(nameof(OrderCompletedPage),  typeof(OrderCompletedPage));
            Routing.RegisterRoute(nameof(OrdersPage),          typeof(OrdersPage));
            Routing.RegisterRoute(nameof(OrderDetailsPage),    typeof(OrderDetailsPage));
            Routing.RegisterRoute(nameof(AccountPage),         typeof(AccountPage));
            Routing.RegisterRoute(nameof(ProfilePage),         typeof(ProfilePage));
            Routing.RegisterRoute(nameof(AdminDashboardPage),  typeof(AdminDashboardPage));
            Routing.RegisterRoute(nameof(ReportsPage),         typeof(ReportsPage));
            Routing.RegisterRoute(nameof(LandingPage),         typeof(LandingPage));
            Routing.RegisterRoute(nameof(MainPage),            typeof(MainPage));
        }
    }
}
