using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;

namespace MauiApp4_elect4.Services
{
    /// <summary>
    /// Provides grocery catalogue data, store vendors, deals, and order management.
    /// </summary>
    public class MockDataService
    {
        // ── Category list ────────────────────────────────────────────────────
        public static readonly List<string> Categories =
        [
            "All", "Fruits", "Vegetables", "Dairy", "Snacks", "Bakery", "Beverages"
        ];

        // ── Valid order statuses ──────────────────────────────────────────────
        public static readonly List<string> OrderStatuses =
        [
            "Out for Delivery", "Preparing Order", "Delivered", "Cancelled"
        ];

        // ── In-memory order store ─────────────────────────────────────────────
        public static readonly ObservableCollection<Order> Orders = [];

        private static int _nextOrderId = 105;

        static MockDataService()
        {
            InitializeSampleOrders();
        }

        public static void InitializeSampleOrders()
        {
            if (Orders.Count > 0) return;

            // 1 · Ongoing: GreenMarket (#104)
            Orders.Add(new Order
            {
                Id = 104,
                VendorName = "GreenMarket",
                VendorIcon = "🌿",
                CustomerName = "Alex Rivera",
                ContactNumber = "+1 (555) 019-2834",
                ShippingAddress = "742 Evergreen Terrace, Springfield, OR 97477",
                DeliveryMethod = "Home Delivery",
                PaymentMethod = "Credit Card",
                Subtotal = 11.99m,
                DeliveryFee = 1.09m,
                TotalAmount = 13.08m,
                Status = "Out for Delivery",
                DeliveryStatusText = "Out for Delivery",
                TabCategory = "Ongoing",
                Line1Preview = "🥬 Fresh Lettuce, Sourdough Bread",
                Line2Preview = "🧃 Orange Juice",
                EstimatedMinutes = 15,
                CourierName = "Mike Roberts",
                CourierPhone = "+1 (555) 839-2041",
                CourierPhotoUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=300&auto=format&fit=crop&q=80",
                Items =
                [
                    new CartItem { Product = new Product { Id = 204, Name = "Fresh Lettuce", Category = "Vegetables", Price = 1.99m, Weight = "1.0g", ImageUrl = "https://images.unsplash.com/photo-1556801712-76c8eb07bbc9?w=500&auto=format&fit=crop&q=80", SubstitutionPreference = SubstitutionOption.AutomaticReplacement }, Quantity = 1, SubstitutionPreference = SubstitutionOption.AutomaticReplacement },
                    new CartItem { Product = new Product { Id = 401, Name = "Sourdough Bread", Category = "Bakery", Price = 1.99m, Weight = "1.0g", ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80", SubstitutionPreference = SubstitutionOption.ContactShopper }, Quantity = 1, SubstitutionPreference = SubstitutionOption.ContactShopper },
                    new CartItem { Product = new Product { Id = 501, Name = "Orange Juice", Category = "Beverages", Price = 1.29m, Weight = "1.0g", ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80", SubstitutionPreference = SubstitutionOption.RefundImmediately }, Quantity = 1, SubstitutionPreference = SubstitutionOption.RefundImmediately }
                ],
                SubstitutionRequests =
                [
                    new PickerSubstitutionRequest
                    {
                        Id = "sub-104-01",
                        OrderId = 104,
                        OriginalItemName = "Sourdough Bread (1.0g)",
                        OriginalItemPrice = 1.99m,
                        ProposedItemName = "Artisan Organic Multigrain Loaf (1.0g)",
                        ProposedItemPrice = 2.49m,
                        PickerName = "Elena Ramos (Store Shopper)",
                        PickerMessage = "Bakery shelf is fresh out of Sourdough Bread. Would you like this freshly baked Organic Multigrain Loaf from Aisle 4 (+ $0.50) as an alternative?",
                        AislePhotoUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=600&auto=format&fit=crop&q=80",
                        Status = SubstitutionStatus.PendingApproval
                    }
                ]
            });

            // 2 · Ongoing: SuperMart (#103)
            Orders.Add(new Order
            {
                Id = 103,
                VendorName = "SuperMart",
                VendorIcon = "🏪",
                CustomerName = "Alex Rivera",
                ContactNumber = "+1 (555) 019-2834",
                ShippingAddress = "742 Evergreen Terrace, Springfield, OR 97477",
                DeliveryMethod = "Home Delivery",
                PaymentMethod = "Digital Wallet",
                Subtotal = 19.99m,
                DeliveryFee = 1.50m,
                TotalAmount = 21.49m,
                Status = "Preparing Order",
                DeliveryStatusText = "Preparing Order",
                TabCategory = "Ongoing",
                Line1Preview = "🍞 Whole Grain Bread",
                Line2Preview = "🥛 Organic Milk",
                EstimatedMinutes = 25,
                CourierName = "Carlos Mendez",
                CourierPhone = "+1 (555) 492-1082",
                CourierPhotoUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&auto=format&fit=crop&q=80",
                Items =
                [
                    new CartItem { Product = new Product { Id = 302, Name = "Whole Grain Bread", Category = "Bakery", Price = 2.99m, Weight = "500g", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop&q=80" }, Quantity = 2 },
                    new CartItem { Product = new Product { Id = 301, Name = "Organic Milk", Category = "Dairy", Price = 3.99m, Weight = "1.0 L", ImageUrl = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=500&auto=format&fit=crop&q=80" }, Quantity = 3 }
                ]
            });

            // 3 · Past: FreshGrocers (#102)
            Orders.Add(new Order
            {
                Id = 102,
                VendorName = "FreshGrocers",
                VendorIcon = "🛒",
                CustomerName = "Alex Rivera",
                ContactNumber = "+1 (555) 019-2834",
                ShippingAddress = "742 Evergreen Terrace, Springfield, OR 97477",
                DeliveryMethod = "Home Delivery",
                PaymentMethod = "Credit Card",
                Subtotal = 17.78m,
                DeliveryFee = 1.09m,
                TotalAmount = 18.87m,
                Status = "Delivered",
                DeliveryStatusText = "Delivered 1 day ago",
                TabCategory = "Past",
                Line1Preview = "🥕 Carrot Bag, Organic Eggs",
                Line2Preview = "🥜 Peanut Butter",
                EstimatedMinutes = 0,
                CourierName = "Sara Jenkins",
                CourierPhone = "+1 (555) 714-3829",
                CourierPhotoUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=300&auto=format&fit=crop&q=80",
                Items =
                [
                    new CartItem { Product = new Product { Id = 202, Name = "Carrot Bag", Category = "Vegetables", Price = 3.49m, Weight = "1.0kg", ImageUrl = "https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?w=500&auto=format&fit=crop&q=80" }, Quantity = 2 },
                    new CartItem { Product = new Product { Id = 102, Name = "Farm Fresh Eggs", Category = "Dairy", Price = 3.99m, Weight = "12 pcs", ImageUrl = "https://images.unsplash.com/photo-1582722872445-44dc5f7e3c8f?w=500&auto=format&fit=crop&q=80" }, Quantity = 2 }
                ]
            });

            // 4 · Past: FreshGrocers (#101)
            Orders.Add(new Order
            {
                Id = 101,
                VendorName = "FreshGrocers",
                VendorIcon = "🛒",
                CustomerName = "Alex Rivera",
                ContactNumber = "+1 (555) 019-2834",
                ShippingAddress = "742 Evergreen Terrace, Springfield, OR 97477",
                DeliveryMethod = "Home Delivery",
                PaymentMethod = "Credit Card",
                Subtotal = 14.31m,
                DeliveryFee = 1.09m,
                TotalAmount = 15.40m,
                Status = "Delivered",
                DeliveryStatusText = "Delivered 3 days ago",
                TabCategory = "Past",
                Line1Preview = "🍎 Organic Apples, Bananas",
                Line2Preview = "🥛 Greek Yogurt",
                EstimatedMinutes = 0,
                CourierName = "David Kim",
                CourierPhone = "+1 (555) 902-3841",
                CourierPhotoUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=300&auto=format&fit=crop&q=80",
                Items =
                [
                    new CartItem { Product = new Product { Id = 101, Name = "Organic Apples", Category = "Fruits", Price = 2.99m, Weight = "1.0kg", ImageUrl = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=500&auto=format&fit=crop&q=80" }, Quantity = 2 }
                ]
            });
        }

        public static void AddOrder(Order order)
        {
            order.Id = _nextOrderId++;
            Orders.Insert(0, order);
        }

        public static Order GetLatestOrder()
        {
            if (Orders.Count == 0) InitializeSampleOrders();
            return Orders.FirstOrDefault() ?? new Order();
        }

        public static Order? GetOrderById(int id)
        {
            if (Orders.Count == 0) InitializeSampleOrders();
            return Orders.FirstOrDefault(o => o.Id == id) ?? GetLatestOrder();
        }

        public static List<Order> GetOrdersByTab(string tab)
        {
            if (Orders.Count == 0) InitializeSampleOrders();

            if (tab.Equals("Past", StringComparison.OrdinalIgnoreCase))
                return Orders.Where(o => o.TabCategory == "Past" || o.Status == "Delivered").ToList();

            if (tab.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                return Orders.Where(o => o.TabCategory == "Cancelled" || o.Status == "Cancelled").ToList();

            // Default: Ongoing
            return Orders.Where(o => o.TabCategory == "Ongoing" || o.Status == "Out for Delivery" || o.Status == "Preparing Order" || o.Status == "Pending").ToList();
        }

        public static void UpdateOrderStatus(Order order, string newStatus)
        {
            if (order is null || string.IsNullOrWhiteSpace(newStatus)) return;
            var idx = Orders.IndexOf(order);
            if (idx < 0) return;

            order.Status = newStatus;
            Orders[idx] = order;
        }

        public static List<Order> GetOrders() => [.. Orders];

        public static void ResetData()
        {
            Orders.Clear();
            _nextOrderId = 105;
            InitializeSampleOrders();
            CartService.Instance.ClearCart();
        }

        // ── Top Vendors ───────────────────────────────────────────────────────
        public List<Vendor> GetTopVendors()
        {
            return
            [
                new Vendor
                {
                    Id = 1,
                    Name = "SuperMart",
                    Rating = 5.0,
                    DeliveryMinutes = 10,
                    Category = "Supermarket",
                    ImageUrl = "https://images.unsplash.com/photo-1578916171728-46686eac8d58?w=500&auto=format&fit=crop&q=80"
                },
                new Vendor
                {
                    Id = 2,
                    Name = "FreshGrocers",
                    Rating = 5.0,
                    DeliveryMinutes = 10,
                    Category = "Organic Market",
                    ImageUrl = "https://images.unsplash.com/photo-1542838132-92c53300491e?w=500&auto=format&fit=crop&q=80"
                },
                new Vendor
                {
                    Id = 3,
                    Name = "GreenMarket",
                    Rating = 4.9,
                    DeliveryMinutes = 15,
                    Category = "Fresh Produce",
                    ImageUrl = "https://images.unsplash.com/photo-1604719312566-8912e9227c6a?w=500&auto=format&fit=crop&q=80"
                }
            ];
        }

        // ── Popular Deals ─────────────────────────────────────────────────────
        public List<Product> GetPopularDeals()
        {
            return
            [
                new Product
                {
                    Id = 101,
                    Name = "Organic Apples",
                    Category = "Fruits",
                    Price = 2.99m,
                    Weight = "1.0kg",
                    Rating = 4.9,
                    StockQuantity = 150,
                    BadgeText = "Deal",
                    Subtitle = "Crisp & Sweet Fuji",
                    ImageUrl = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=500&auto=format&fit=crop&q=80"
                },
                new Product
                {
                    Id = 102,
                    Name = "Farm Fresh Eggs",
                    Category = "Dairy",
                    Price = 3.99m,
                    Weight = "12 pcs",
                    Rating = 5.0,
                    StockQuantity = 80,
                    BadgeText = "+09",
                    Subtitle = "Cage-free Organic",
                    ImageUrl = "https://images.unsplash.com/photo-1582722872445-44dc5f7e3c8f?w=500&auto=format&fit=crop&q=80"
                },
                new Product
                {
                    Id = 103,
                    Name = "Fresh Strawberries",
                    Category = "Fruits",
                    Price = 2.99m,
                    Weight = "250g",
                    Rating = 4.8,
                    StockQuantity = 90,
                    BadgeText = "-20%",
                    Subtitle = "Sweet Garden Picked",
                    ImageUrl = "https://images.unsplash.com/photo-1464965911861-746a04b4bca6?w=500&auto=format&fit=crop&q=80"
                },
                new Product
                {
                    Id = 104,
                    Name = "Ripe Hass Avocados",
                    Category = "Vegetables",
                    Price = 2.49m,
                    Weight = "3 pcs",
                    Rating = 4.9,
                    StockQuantity = 110,
                    BadgeText = "Fresh",
                    Subtitle = "Creamy & Ready",
                    ImageUrl = "https://images.unsplash.com/photo-1523049673857-eb18f1d7b578?w=500&auto=format&fit=crop&q=80"
                }
            ];
        }

        // ── Fresh Vegetables (GreenMarket) ────────────────────────────────────
        public List<Product> GetFreshVegetables()
        {
            return
            [
                new Product
                {
                    Id = 201,
                    Name = "Broccoli",
                    Category = "Vegetables",
                    Price = 2.99m,
                    Weight = "500g",
                    Rating = 4.9,
                    StockQuantity = 60,
                    BadgeText = "Fresh",
                    Subtitle = "Organic Green Crown",
                    ImageUrl = "https://images.unsplash.com/photo-1459411621453-7b03977f4bfc?w=500&auto=format&fit=crop&q=80"
                },
                new Product
                {
                    Id = 202,
                    Name = "Carrot Bag",
                    Category = "Vegetables",
                    Price = 3.49m,
                    Weight = "1.0kg",
                    Rating = 4.8,
                    StockQuantity = 85,
                    BadgeText = "Crisp",
                    Subtitle = "Farm Root Carrots",
                    ImageUrl = "https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?w=500&auto=format&fit=crop&q=80"
                },
                new Product
                {
                    Id = 203,
                    Name = "Cherry Tomato",
                    Category = "Vegetables",
                    Price = 1.99m,
                    Weight = "300g",
                    Rating = 5.0,
                    StockQuantity = 120,
                    BadgeText = "Sweet",
                    Subtitle = "Vine Ripened Ruby",
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=500&auto=format&fit=crop&q=80"
                }
            ];
        }

        // ── Bestsellers (GreenMarket) ──────────────────────────────────────────
        public List<Product> GetBestsellers()
        {
            return
            [
                new Product
                {
                    Id = 301,
                    Name = "Organic Milk",
                    Category = "Dairy",
                    Price = 3.99m,
                    Weight = "1.0 L",
                    Rating = 5.0,
                    StockQuantity = 95,
                    BadgeText = "Bestseller",
                    Subtitle = "Whole Pure Farm Milk",
                    ImageUrl = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=500&auto=format&fit=crop&q=80"
                },
                new Product
                {
                    Id = 302,
                    Name = "Whole Grain Bread",
                    Category = "Bakery",
                    Price = 2.99m,
                    Weight = "500g",
                    Rating = 4.9,
                    StockQuantity = 45,
                    BadgeText = "Baked Daily",
                    Subtitle = "Artisan Stone Oven",
                    ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop&q=80"
                }
            ];
        }

        // ── Master product catalogue ─────────────────────────────────────────
        public List<Product> GetProducts()
        {
            return
            [
                // Fruits
                new Product { Id = 101, Name = "Organic Apples", Category = "Fruits", Price = 2.99m, Weight = "1.0kg", StockQuantity = 150, ImageUrl = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 103, Name = "Fresh Strawberries", Category = "Fruits", Price = 2.99m, Weight = "250g", StockQuantity = 90, ImageUrl = "https://images.unsplash.com/photo-1464965911861-746a04b4bca6?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 105, Name = "Bananas Bundle", Category = "Fruits", Price = 1.49m, Weight = "1.0kg", StockQuantity = 180, ImageUrl = "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 106, Name = "Sweet Oranges", Category = "Fruits", Price = 2.29m, Weight = "1.0kg", StockQuantity = 110, ImageUrl = "https://images.unsplash.com/photo-1611080626919-7cf5a9dbab5b?w=500&auto=format&fit=crop&q=80" },

                // Vegetables
                new Product { Id = 201, Name = "Broccoli", Category = "Vegetables", Price = 2.99m, Weight = "500g", StockQuantity = 60, ImageUrl = "https://images.unsplash.com/photo-1459411621453-7b03977f4bfc?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 202, Name = "Carrot Bag", Category = "Vegetables", Price = 3.49m, Weight = "1.0kg", StockQuantity = 85, ImageUrl = "https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 203, Name = "Cherry Tomato", Category = "Vegetables", Price = 1.99m, Weight = "300g", StockQuantity = 120, ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 204, Name = "Fresh Lettuce", Category = "Vegetables", Price = 1.99m, Weight = "1.0g", StockQuantity = 75, ImageUrl = "https://images.unsplash.com/photo-1556801712-76c8eb07bbc9?w=500&auto=format&fit=crop&q=80" },

                // Dairy
                new Product { Id = 102, Name = "Farm Fresh Eggs", Category = "Dairy", Price = 3.99m, Weight = "12 pcs", StockQuantity = 80, ImageUrl = "https://images.unsplash.com/photo-1582722872445-44dc5f7e3c8f?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 301, Name = "Organic Milk", Category = "Dairy", Price = 3.99m, Weight = "1.0 L", StockQuantity = 95, ImageUrl = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 303, Name = "Greek Yogurt Plain", Category = "Dairy", Price = 2.49m, Weight = "500g", StockQuantity = 85, ImageUrl = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 304, Name = "Cheddar Cheese Block", Category = "Dairy", Price = 3.79m, Weight = "250g", StockQuantity = 60, ImageUrl = "https://images.unsplash.com/photo-1618164436241-4473940d1f5c?w=500&auto=format&fit=crop&q=80" },

                // Bakery
                new Product { Id = 401, Name = "Sourdough Bread", Category = "Bakery", Price = 1.99m, Weight = "1.0g", StockQuantity = 40, ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 302, Name = "Whole Grain Bread", Category = "Bakery", Price = 2.99m, Weight = "500g", StockQuantity = 45, ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 402, Name = "Butter Croissants", Category = "Bakery", Price = 2.59m, Weight = "2 pcs", StockQuantity = 35, ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=500&auto=format&fit=crop&q=80" },

                // Beverages
                new Product { Id = 501, Name = "Orange Juice", Category = "Beverages", Price = 1.29m, Weight = "1.0g", StockQuantity = 95, ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 502, Name = "Still Mineral Water", Category = "Beverages", Price = 0.89m, Weight = "1.5 L", StockQuantity = 300, ImageUrl = "https://images.unsplash.com/photo-1548839140-29a749e1bc4e?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 503, Name = "Green Tea", Category = "Beverages", Price = 2.29m, Weight = "20 bags", StockQuantity = 150, ImageUrl = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?w=500&auto=format&fit=crop&q=80" },

                // Snacks
                new Product { Id = 601, Name = "Roasted Almonds", Category = "Snacks", Price = 3.99m, Weight = "200g", StockQuantity = 70, ImageUrl = "https://images.unsplash.com/photo-1508061253366-f7da158b6d46?w=500&auto=format&fit=crop&q=80" },
                new Product { Id = 602, Name = "Organic Granola Bar", Category = "Snacks", Price = 1.79m, Weight = "6 pcs", StockQuantity = 90, ImageUrl = "https://images.unsplash.com/photo-1622484216806-039c9f28122d?w=500&auto=format&fit=crop&q=80" }
            ];
        }

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
