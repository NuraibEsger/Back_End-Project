using BackEndProject.Data;
using BackEndProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BackEndProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index(int productNumber, int pagenumber, string sortOrder)
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

            var products = _dbContext.Products.Include(x=>x.ProductImages).AsNoTracking().ToList();

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

            var pagedProducts = products.Skip((page - 1) * defaultTake).Take(defaultTake).ToList();

            var model = new HomeIndexVM
            {
                Products = products
            };
            return View(model);
        }
    }
}