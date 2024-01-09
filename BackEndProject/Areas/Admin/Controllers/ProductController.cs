using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entities;
using BackEndProject.Entity;
using BackEndProject.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using NETCore.MailKit.Core;
using System.Net.Mail;
using WebApplication1.Services;

namespace BackEndProject.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly AppDbContext _dbContext;
        private readonly FileService _fileService;
        private readonly SendEmail _sendEmail;
        public ProductController(AppDbContext dbContext, FileService fileService, SendEmail sendEmail)
        {
            _dbContext = dbContext;
            _fileService = fileService;
            _sendEmail = sendEmail;
        }
        public IActionResult Index()
        {
            var product = _dbContext.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Category)
                .Include(x=>x.Brand)
                .Include(x=>x.Color)
                .ToList();

            var model = new ProductIndexVM
            {
                Products = product,
            };
            return View(model);
        }
        public IActionResult Add()
        {
            var model = new ProductAddVM();

            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var colors = _dbContext.Colors.AsNoTracking().ToList();
            var brands = _dbContext.Brands.AsNoTracking().ToList();

            model.Categories = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            model.Colors = colors.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            model.Brands = brands.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ProductAddVM model)
        {
            if (!ModelState.IsValid)
            {
                var categories = _dbContext.Categories.AsNoTracking().ToList();
                var colors = _dbContext.Colors.AsNoTracking().ToList();
                var brands = _dbContext.Brands.AsNoTracking().ToList();

                model.Categories = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                model.Colors = colors.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                model.Brands = brands.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                return View(model);
            }

            var imageNames = new List<string>();
            foreach (var img in model.Photos)
            {
                var imageName = _fileService.UploadFile(img);
                imageNames.Add(imageName);
            }

            var productImages = imageNames.Select(imageName => new ProductImage
            {
                ImagePath = imageName
            }).ToList();

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Count = model.Count,
                CategoryId = model.CategoryId,
                ColorId = model.ColorId,
                BrandId = model.BrandId,
                ProductImages = productImages
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            var text = $"{product.Name} is added";
            var subscribedUsers = _dbContext.Emails.AsNoTracking().ToList();

            foreach (var user in subscribedUsers)
            {
                bool isSend = await _sendEmail.EmailSend(user.Email, text);
            }
            return RedirectToAction("Index");
        }
        
        public IActionResult Update(int id)
        {
            var product = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Color)
                .Include(x => x.ProductImages)
                .FirstOrDefault(x => x.Id == id);

            if (product is null) return NotFound();

            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var brands = _dbContext.Brands.AsNoTracking().ToList();
            var colors = _dbContext.Colors.AsNoTracking().ToList();

            var model = new ProductUpdateVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Count = product.Count,
                CategoryId = product.Category!.Id,
                ColorId = product.Color!.Id,
                BrandId = product.BrandId,
                ProductImages = product.ProductImages,
                Categories = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }).ToList(),
                Brands = brands.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }).ToList(),

                Colors = colors.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }).ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(ProductUpdateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = _dbContext.Products
                .Include(x => x.Category)
                .Include(x=>x.Brand)
                .Include(x=>x.Color)
                .Include(x => x.ProductImages)
                .FirstOrDefault(x => x.Id == model.Id);

            if (product is null) return NotFound();

            foreach (var item in product.ProductImages)
            {
                if (model.Photo != null)
                {
                    if (item != null && !model.Photo.Any(p => p.FileName == item.ImagePath))
                    {
                        _fileService.DeleteFile(item.ImagePath!);
                        _dbContext.ProductImages.Remove(item);
                    }
                }
            }

            foreach (var photo in model.Photo ?? Enumerable.Empty<IFormFile>())
            {
                var productImage = new ProductImage
                {
                    ImagePath = _fileService.UploadFile(photo)
                };
                product.ProductImages.Add(productImage);
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Count = model.Count;
            product.CategoryId = model.CategoryId;
            product.BrandId = model.BrandId;
            product.ColorId = model.ColorId;

            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.Id == id);

            if (product is null) NotFound();

            _dbContext.Products.Remove(product!);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
