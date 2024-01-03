using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly AppDbContext? _dbContext;
        public CategoryController(AppDbContext dbContext)
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
        public IActionResult Add()
        {
            var model = new CategoryAddVM();

            return View(model);
        }
        [HttpPost]
        public IActionResult Add(CategoryAddVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_dbContext.Categories.Any(c => c.Name == model.CategoryName))
            {
                ModelState.AddModelError("Category", "Category already exists");
                return View(model);
            }

            var category = new Category
            {
                Name = model.CategoryName,
            };

            _dbContext.Categories.Add(category);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category is null) return NotFound();

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category is null) return NotFound();

            var model = new CategoryUpdateVM
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Update(CategoryUpdateVM model)
        {
            if (model is null) return NotFound();

            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == model.Id);

            if (category is null) return NotFound();

            category.Name = model.Name;

            _dbContext.Categories.Update(category);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
