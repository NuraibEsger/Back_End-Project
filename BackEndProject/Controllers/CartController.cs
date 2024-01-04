using BackEndProject.Data;
using BackEndProject.Entities;
using BackEndProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackEndProject.Controllers
{
    public class CartController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly AppDbContext _dbContext;

        public CartController(ShoppingCart shoppingCart, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _shoppingCart = shoppingCart;
        }
        public IActionResult Index(int productId, int quantity)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _dbContext.Products.Find(productId);

            if (product is null)
            {
                return NotFound();
            }

            var shoppingCart = _dbContext.Carts
                .Include(x=>x.CartItems)
                .FirstOrDefault(x => x.UserId == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new Cart { UserId = userId };
                _dbContext.Carts.Add(shoppingCart);
            }

            var cartItem = shoppingCart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

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
            return RedirectToAction("Index", "Home");
        }
    }
}
