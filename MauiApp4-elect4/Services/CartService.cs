using System.Collections.ObjectModel;
using System.ComponentModel;
using MauiApp4_elect4.Models;

namespace MauiApp4_elect4.Services
{
    /// <summary>
    /// Singleton service that owns the shopping cart state for the entire session.
    /// UI layers bind directly to <see cref="CartItems"/> and <see cref="CartUpdated"/>
    /// so the UI updates automatically whenever items or quantities change.
    /// </summary>
    public class CartService : INotifyPropertyChanged
    {
        // ── Singleton ────────────────────────────────────────────────────────
        private static readonly Lazy<CartService> _instance =
            new(() => new CartService(), isThreadSafe: true);

        /// <summary>The one shared instance of the cart for the app lifetime.</summary>
        public static CartService Instance => _instance.Value;

        public event EventHandler? CartUpdated;
        public event PropertyChangedEventHandler? PropertyChanged;

        // Private constructor — initialize demo items matching reference design
        private CartService()
        {
            CartItems.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (CartItem item in e.NewItems)
                    {
                        item.PropertyChanged -= OnItemPropertyChanged;
                        item.PropertyChanged += OnItemPropertyChanged;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (CartItem item in e.OldItems)
                    {
                        item.PropertyChanged -= OnItemPropertyChanged;
                    }
                }
                NotifyCartChanged();
            };

            InitializeDemoCart();
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyCartChanged();
        }

        private void NotifyCartChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CartItems)));
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }

        // ── State ────────────────────────────────────────────────────────────
        /// <summary>
        /// Live collection of items in the cart.
        /// </summary>
        public ObservableCollection<CartItem> CartItems { get; } = [];

        public void InitializeDemoCart()
        {
            if (CartItems.Count > 0) return;

            CartItems.Add(new CartItem
            {
                Product = new Product
                {
                    Id = 401,
                    Name = "Sourdough Bread",
                    Category = "Bakery",
                    Price = 1.99m,
                    Weight = "1.0g",
                    StockQuantity = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80"
                },
                Quantity = 1
            });

            CartItems.Add(new CartItem
            {
                Product = new Product
                {
                    Id = 204,
                    Name = "Fresh Lettuce",
                    Category = "Vegetables",
                    Price = 1.99m,
                    Weight = "1.0g",
                    StockQuantity = 75,
                    ImageUrl = "https://images.unsplash.com/photo-1556801712-76c8eb07bbc9?w=500&auto=format&fit=crop&q=80"
                },
                Quantity = 1
            });

            CartItems.Add(new CartItem
            {
                Product = new Product
                {
                    Id = 501,
                    Name = "Orange Juice",
                    Category = "Beverages",
                    Price = 1.29m,
                    Weight = "1.0g",
                    StockQuantity = 95,
                    ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80"
                },
                Quantity = 1
            });
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Adds one unit of <paramref name="product"/> to the cart.
        /// If the product is already in the cart its quantity is incremented.
        /// </summary>
        public void AddToCart(Product product)
        {
            if (product is null) return;

            var existing = CartItems.FirstOrDefault(
                ci => ci.Product?.Id == product.Id);

            if (existing is not null)
            {
                existing.Quantity++;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1,
                    SubstitutionPreference = product.SubstitutionPreference,
                    FallbackProductId = product.FallbackProductId
                });
            }
        }

        /// <summary>
        /// Increments the quantity of an existing cart item by 1.
        /// </summary>
        public void IncrementQuantity(CartItem item)
        {
            if (item is null) return;
            item.Quantity++;
        }

        /// <summary>
        /// Decrements the quantity of an existing cart item by 1.
        /// Automatically removes the item when quantity reaches zero.
        /// </summary>
        public void DecrementQuantity(CartItem item)
        {
            if (item is null) return;

            if (item.Quantity <= 1)
            {
                CartItems.Remove(item);
                return;
            }

            item.Quantity--;
        }

        /// <summary>Removes a specific cart item regardless of its quantity.</summary>
        public void RemoveFromCart(CartItem item)
        {
            if (item is null) return;
            CartItems.Remove(item);
        }

        /// <summary>Empties the entire cart.</summary>
        public void ClearCart()
        {
            if (MainThread.IsMainThread)
            {
                CartItems.Clear();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => CartItems.Clear());
            }
        }

        /// <summary>Returns the grand total of all line-item subtotals.</summary>
        public decimal GetTotalAmount() =>
            CartItems.Sum(ci => ci.Subtotal);

        /// <summary>Returns the number of individual units across all cart items.</summary>
        public int GetTotalItemCount() =>
            CartItems.Sum(ci => ci.Quantity);
    }
}
