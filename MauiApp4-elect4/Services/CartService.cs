using System.Collections.ObjectModel;
using MauiApp4_elect4.Models;

namespace MauiApp4_elect4.Services
{
    /// <summary>
    /// Singleton service that owns the shopping cart state for the entire session.
    /// UI layers bind directly to <see cref="CartItems"/> so the UI updates automatically
    /// whenever the collection changes.
    /// </summary>
    public class CartService
    {
        // ── Singleton ────────────────────────────────────────────────────────
        private static readonly Lazy<CartService> _instance =
            new(() => new CartService(), isThreadSafe: true);

        /// <summary>The one shared instance of the cart for the app lifetime.</summary>
        public static CartService Instance => _instance.Value;

        // Private constructor — use Instance property.
        private CartService() { }

        // ── State ────────────────────────────────────────────────────────────
        /// <summary>
        /// Live collection of items in the cart.
        /// Bind CollectionViews or Labels directly to this — changes propagate automatically.
        /// </summary>
        public ObservableCollection<CartItem> CartItems { get; } = [];

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
                // ObservableCollection doesn't detect property changes on items —
                // replace the item so the UI refreshes.
                var idx = CartItems.IndexOf(existing);
                CartItems[idx] = existing;
            }
            else
            {
                CartItems.Add(new CartItem { Product = product, Quantity = 1 });
            }
        }

        /// <summary>
        /// Increments the quantity of an existing cart item by 1.
        /// </summary>
        public void IncrementQuantity(CartItem item)
        {
            if (item is null) return;
            var idx = CartItems.IndexOf(item);
            if (idx < 0) return;

            item.Quantity++;
            CartItems[idx] = item;   // trigger ObservableCollection change
        }

        /// <summary>
        /// Decrements the quantity of an existing cart item by 1.
        /// Automatically removes the item when quantity reaches zero.
        /// </summary>
        public void DecrementQuantity(CartItem item)
        {
            if (item is null) return;
            var idx = CartItems.IndexOf(item);
            if (idx < 0) return;

            if (item.Quantity <= 1)
            {
                CartItems.RemoveAt(idx);
                return;
            }

            item.Quantity--;
            CartItems[idx] = item;   // trigger ObservableCollection change
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
