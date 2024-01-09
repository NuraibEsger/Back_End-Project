using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
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
        public IActionResult Index(int? categoryId, int? brandId, int? colorId)
        {
            var product = _dbContext.Products
                .Where(x=> (categoryId == null ? true : x.CategoryId == categoryId)
                && (brandId == null ? true : x.BrandId == brandId)
                && (colorId == null ? true : x.ColorId == colorId))
                .Include(x=>x.ProductImages).AsNoTracking().ToList();
            var category = _dbContext.Categories.AsNoTracking().ToList();
            var color = _dbContext.Colors.AsNoTracking().ToList();
            var brand = _dbContext.Brands.AsNoTracking().ToList();

            var model = new ShopIndexVM
            {
                Products = product,
                Colors = color,
                Brands = brand,
                Categories = category,
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
