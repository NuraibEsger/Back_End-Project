using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : BaseController
    {
        private readonly AppDbContext _dbContext;
        public BrandController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var brands = _dbContext.Brands.AsNoTracking().ToList();

            var model = new BrandIndexVM
            {
                Brands = brands
            };

            return View(model);
        }
        public IActionResult Add()
        {
            var model = new BrandAddVM();

            return View(model);
        }
        [HttpPost]
        public IActionResult Add(BrandAddVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_dbContext.Brands.Any(c => c.Name == model.Name))
            {
                ModelState.AddModelError("Category", "Category already exists");
                return View(model);
            }

            var brand = new Brand
            {
                Name = model.Name,
            };

            _dbContext.Brands.Add(brand);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var brand = _dbContext.Brands.FirstOrDefault(x => x.Id == id);
            if (brand is null) return NotFound();

            _dbContext.Brands.Remove(brand);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            var brand = _dbContext.Brands.FirstOrDefault(x => x.Id == id);
            if (brand is null) return NotFound();

            var model = new BrandUpdateVM
            {
                Id = brand.Id,
                Name = brand.Name
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Update(BrandUpdateVM model)
        {
            if (model is null) return NotFound();

            var brand = _dbContext.Brands.FirstOrDefault(x => x.Id == model.Id);

            if (brand is null) return NotFound();

            brand.Name = model.Name;

            _dbContext.Brands.Update(brand);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
