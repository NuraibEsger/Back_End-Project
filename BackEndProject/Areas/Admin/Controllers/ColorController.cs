using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ColorController : BaseController
    {
        private readonly AppDbContext _dbContext;

        public ColorController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var colors = _dbContext.Colors.AsNoTracking().ToList();

            var model = new ColorIndexVM
            {
                Colors = colors
            };

            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Update(int? id)
        {
            if (id is null) return NotFound();

            var color = _dbContext.Colors.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (color is null) return NotFound();

            var model = new ColorUpdateVM
            {
                Id = color.Id,
                Name = color.Name,
                Code = color.Code
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(ColorUpdateVM model)
        {
            var color = _dbContext.Colors.FirstOrDefault(x => x.Id == model.Id);
            if (color is null) return NotFound();

            color.Name = model.Name;
            color.Code = model.Code;

            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Add(ColorAddVM model)
        {
            var color = new Color
            {
                Name = model.Name,
                Code = model.Code
            };

            _dbContext.Colors.Add(color);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id is null) return NotFound();

            var color = _dbContext.Colors.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (color is null) return NotFound();

            _dbContext.Colors.Remove(color);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
