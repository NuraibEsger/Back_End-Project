using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
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
        public IActionResult Index()
        {
            var category = _dbContext.Categories.AsNoTracking().ToList();

            var model = new CategoryIndexVM
            {
                Categories = category
            };
            return View(model);
        }
    }
}
