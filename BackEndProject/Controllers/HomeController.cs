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
        public IActionResult Index()
        {
            var products = _dbContext.Products.AsNoTracking().ToList();
            var model = new HomeIndexVM
            {
                Products = products
            };
            return View(model);
        }
    }
}