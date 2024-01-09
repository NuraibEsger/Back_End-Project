using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entities;
using BackEndProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _dbContext;
        public ShopController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(int? categoryId, int? brandId, int? colorId, string sortOrder)
        {
            var products = _dbContext.Products
                .Where(x=> (categoryId == null ? true : x.CategoryId == categoryId)
                && (brandId == null ? true : x.BrandId == brandId)
                && (colorId == null ? true : x.ColorId == colorId))
                .Include(x=>x.ProductImages).AsNoTracking().ToList();
            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var colors = _dbContext.Colors.AsNoTracking().ToList();
            var brands = _dbContext.Brands.AsNoTracking().ToList();

            switch (sortOrder)
            {
                case "Ascending":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "Descending":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                default:
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
            }

            var model = new ShopIndexVM
            {
                Products = products,
                Colors = colors,
                Brands = brands,
                Categories = categories,
            };
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var product = _dbContext.Products.Include(x=>x.ProductImages).AsNoTracking().ToList();
            if (product == null) { return NotFound(); }

            var model = new ShopDetailVM
            {
                Product = product.FirstOrDefault(x => x.Id == id),
            };

            return View(model);
        }
    }
}
