using BackEndProject.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.Admin.Controllers
{
    public class BrandController : BaseController
    {
        private readonly AppDbContext _dbContext;
        public BrandController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {

            return View();
        }
        
    }
}
