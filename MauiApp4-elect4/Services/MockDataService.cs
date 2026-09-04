using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;

namespace MauiApp4_elect4.Services
{
    /// <summary>
    /// Provides hard-coded sample grocery data for UI prototyping and testing.
    /// Replace with a real API / database service in production.
    /// </summary>
    public class MockDataService
    {
        // ── Category list ────────────────────────────────────────────────────
        /// <summary>All available filter categories shown in the horizontal pill bar.</summary>
        public static readonly List<string> Categories =
        [
            "All", "Dairy", "Bakery", "Fruits", "Beverages"
        ];

        // ── Valid order statuses ──────────────────────────────────────────────
        /// <summary>Ordered list of fulfillment statuses used in the Admin Dashboard picker.</summary>
        public static readonly List<string> OrderStatuses =
        [
            "Pending", "Processing", "Out for Delivery", "Delivered"
        ];

        // ── In-memory order store ─────────────────────────────────────────────
        /// <summary>
        /// Shared, observable list of all placed orders.
        /// Both CheckoutPage (writes) and AdminDashboardPage (reads/updates) use this.
        /// </summary>
        public static readonly ObservableCollection<Order> Orders = [];

        private static int _nextOrderId = 1;

        /// <summary>Persists a new order and auto-assigns its Id.</summary>
        public static void AddOrder(Order order)
        {
            order.Id = _nextOrderId++;
            Orders.Add(order);
        }

        /// <summary>Updates the status of an existing order in place.</summary>
        public static void UpdateOrderStatus(Order order, string newStatus)
        {
            if (order is null || string.IsNullOrWhiteSpace(newStatus)) return;
            var idx = Orders.IndexOf(order);
            if (idx < 0) return;

            order.Status = newStatus;
            // Replace item to force ObservableCollection notification.
            Orders[idx] = order;
        }

        /// <summary>Returns a snapshot list of all current orders.</summary>
        public static List<Order> GetOrders() => [.. Orders];

        /// <summary>
        /// Resets all mutable application state to the initial demo configuration:
        /// clears every placed order, resets the auto-increment order ID counter,
        /// and empties the shopping cart.
        /// Call this from the Admin dashboard "Reset Application Data" button
        /// before a fresh presentation run.
        /// </summary>
        public static void ResetData()
        {
            // 1. Clear all placed orders
            Orders.Clear();
            _nextOrderId = 1;

            // 2. Empty the shopping cart
            CartService.Instance.ClearCart();
        }

        // ── Master product catalogue ─────────────────────────────────────────
        /// <summary>Returns the full catalogue of grocery products.</summary>
        public List<Product> GetProducts()
        {
            return
            [
                // ── Dairy ────────────────────────────────────────────────────
                new Product { Id =  1, Name = "Fresh Full-Cream Milk (1 L)",     Category = "Dairy",     Price = 1.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 120 },
                new Product { Id =  2, Name = "Low-Fat Milk (2 L)",              Category = "Dairy",     Price = 2.79m,  ImageUrl = "dotnet_bot.png", StockQuantity = 95  },
                new Product { Id =  3, Name = "Greek Yogurt Plain (500 g)",      Category = "Dairy",     Price = 2.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 85  },
                new Product { Id =  4, Name = "Strawberry Yogurt (150 g)",       Category = "Dairy",     Price = 1.09m,  ImageUrl = "dotnet_bot.png", StockQuantity = 110 },
                new Product { Id =  5, Name = "Cheddar Cheese Block (250 g)",    Category = "Dairy",     Price = 3.79m,  ImageUrl = "dotnet_bot.png", StockQuantity = 60  },
                new Product { Id =  6, Name = "Mozzarella Shredded (200 g)",     Category = "Dairy",     Price = 3.29m,  ImageUrl = "dotnet_bot.png", StockQuantity = 75  },
                new Product { Id =  7, Name = "Salted Butter (250 g)",           Category = "Dairy",     Price = 2.89m,  ImageUrl = "dotnet_bot.png", StockQuantity = 90  },
                new Product { Id =  8, Name = "Whipping Cream (200 ml)",         Category = "Dairy",     Price = 2.19m,  ImageUrl = "dotnet_bot.png", StockQuantity = 50  },

                // ── Bakery ───────────────────────────────────────────────────
                new Product { Id =  9, Name = "Whole Wheat Bread (400 g)",       Category = "Bakery",    Price = 2.29m,  ImageUrl = "dotnet_bot.png", StockQuantity = 45  },
                new Product { Id = 10, Name = "White Sandwich Loaf",             Category = "Bakery",    Price = 1.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 60  },
                new Product { Id = 11, Name = "Sourdough Loaf (500 g)",          Category = "Bakery",    Price = 3.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 30  },
                new Product { Id = 12, Name = "Blueberry Muffins (4-pack)",      Category = "Bakery",    Price = 3.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 25  },
                new Product { Id = 13, Name = "Butter Croissants (2-pack)",      Category = "Bakery",    Price = 2.59m,  ImageUrl = "dotnet_bot.png", StockQuantity = 40  },
                new Product { Id = 14, Name = "Sesame Bagels (4-pack)",          Category = "Bakery",    Price = 2.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 35  },
                new Product { Id = 15, Name = "Chocolate Chip Cookies (200 g)",  Category = "Bakery",    Price = 2.79m,  ImageUrl = "dotnet_bot.png", StockQuantity = 55  },
                new Product { Id = 16, Name = "Cinnamon Rolls (6-pack)",         Category = "Bakery",    Price = 4.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 20  },

                // ── Fruits ───────────────────────────────────────────────────
                new Product { Id = 17, Name = "Red Apples (1 kg)",               Category = "Fruits",    Price = 1.79m,  ImageUrl = "dotnet_bot.png", StockQuantity = 200 },
                new Product { Id = 18, Name = "Green Grapes (500 g)",            Category = "Fruits",    Price = 2.39m,  ImageUrl = "dotnet_bot.png", StockQuantity = 130 },
                new Product { Id = 19, Name = "Bananas (bunch ~6)",              Category = "Fruits",    Price = 0.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 180 },
                new Product { Id = 20, Name = "Watermelon (whole)",              Category = "Fruits",    Price = 5.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 40  },
                new Product { Id = 21, Name = "Strawberries (250 g punnet)",     Category = "Fruits",    Price = 2.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 90  },
                new Product { Id = 22, Name = "Mangoes (2-pack)",                Category = "Fruits",    Price = 3.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 70  },
                new Product { Id = 23, Name = "Pineapple (whole)",               Category = "Fruits",    Price = 2.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 55  },
                new Product { Id = 24, Name = "Blueberries (125 g punnet)",      Category = "Fruits",    Price = 3.19m,  ImageUrl = "dotnet_bot.png", StockQuantity = 80  },

                // ── Beverages ────────────────────────────────────────────────
                new Product { Id = 25, Name = "Still Mineral Water (1.5 L)",     Category = "Beverages", Price = 0.89m,  ImageUrl = "dotnet_bot.png", StockQuantity = 300 },
                new Product { Id = 26, Name = "Sparkling Water (1 L)",           Category = "Beverages", Price = 1.19m,  ImageUrl = "dotnet_bot.png", StockQuantity = 200 },
                new Product { Id = 27, Name = "Fresh Orange Juice (1 L)",        Category = "Beverages", Price = 2.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 95  },
                new Product { Id = 28, Name = "Apple Juice (1 L)",               Category = "Beverages", Price = 2.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 110 },
                new Product { Id = 29, Name = "Oat Milk (1 L)",                  Category = "Beverages", Price = 2.89m,  ImageUrl = "dotnet_bot.png", StockQuantity = 85  },
                new Product { Id = 30, Name = "Almond Milk (1 L)",               Category = "Beverages", Price = 3.19m,  ImageUrl = "dotnet_bot.png", StockQuantity = 70  },
                new Product { Id = 31, Name = "Green Tea (20 bags)",             Category = "Beverages", Price = 2.29m,  ImageUrl = "dotnet_bot.png", StockQuantity = 150 },
                new Product { Id = 32, Name = "Instant Coffee (200 g jar)",      Category = "Beverages", Price = 4.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 60  },
                new Product { Id = 33, Name = "Energy Drink (250 ml)",           Category = "Beverages", Price = 1.49m,  ImageUrl = "dotnet_bot.png", StockQuantity = 140 },
                new Product { Id = 34, Name = "Lemonade Sparkling (330 ml can)", Category = "Beverages", Price = 0.99m,  ImageUrl = "dotnet_bot.png", StockQuantity = 220 },
            ];
        }

        /// <summary>
        /// Filters by category and/or search term.
        /// Pass <c>null</c> / empty strings to skip each filter.
        /// </summary>
        public List<Product> GetFilteredProducts(string? category, string? searchTerm)
        {
            var query = GetProducts().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category) &&
                !category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p =>
                    p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }
    }
}
