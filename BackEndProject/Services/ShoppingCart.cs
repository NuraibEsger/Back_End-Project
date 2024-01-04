using BackEndProject.Data;
using BackEndProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Services
{
    public class ShoppingCart
    {
        private readonly AppDbContext _dbContext;
        public ShoppingCart(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<CartItem> Items { get; } = new();

        public void AddToCart(int productId, int quantity, string userId)
        {
            var product = _dbContext.Products.Find(productId);

            if (product == null)
            {
                return;
            }

            var shoppingCart = _dbContext.Carts
                .Include(x => x.CartItems)
                .FirstOrDefault(x => x.UserId == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new Cart { UserId = userId };
                _dbContext.Carts.Add(shoppingCart);
            }

            var cartItem = shoppingCart.CartItems.FirstOrDefault(x => x.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem { ProductId = productId, Quantity = quantity };
                shoppingCart.CartItems.Add(cartItem);
            }

            _dbContext.SaveChanges();
        }
        public void RemoveFromCart(int productId)
        {
            var itemToRemove = Items.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }
    }
}
