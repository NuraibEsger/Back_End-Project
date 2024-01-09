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
        public IActionResult Index(string sortOrder)
        {
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

            var model = new HomeIndexVM
            {
                Products = products
            };
            return View(model);
        }
    }
}