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
        public IActionResult Index(int productNumber, int pagenumber, int? categoryId, int? brandId, int? colorId, string sortOrder)
        {
            if (pagenumber <= 0) pagenumber = 1;
            var defaultTake = 3;
            if (productNumber != 0)
            {
                defaultTake = productNumber;
            }
            var page = pagenumber;

            decimal productCount = _dbContext.Products.Count();
            var pageCount = (int)Math.Ceiling(productCount / defaultTake);


            var products = _dbContext.Products
                .Where(x=> (categoryId == null ? true : x.CategoryId == categoryId)
                && (brandId == null ? true : x.BrandId == brandId)
                && (colorId == null ? true : x.ColorId == colorId))
                .Include(x=>x.ProductImages).AsNoTracking().ToList();
            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var colors = _dbContext.Colors.AsNoTracking().ToList();
            var brands = _dbContext.Brands.AsNoTracking().ToList();

            var pagedProducts = products.Skip((page - 1) * defaultTake).Take(defaultTake).ToList();

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
