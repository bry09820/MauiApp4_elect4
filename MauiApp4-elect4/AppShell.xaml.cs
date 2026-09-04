using MauiApp4_elect4.Views;

namespace MauiApp4_elect4
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // ── Push/modal routes (pages not directly in the TabBar) ──────────
            // These are reached via Shell.Current.GoToAsync(nameof(XPage)).
            Routing.RegisterRoute(nameof(CartPage),           typeof(CartPage));
            Routing.RegisterRoute(nameof(CheckoutPage),       typeof(CheckoutPage));
            Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
            Routing.RegisterRoute(nameof(ProfilePage),        typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ReportsPage),        typeof(ReportsPage));
        }
    }
}
