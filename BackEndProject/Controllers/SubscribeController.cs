using BackEndProject.Data;
using BackEndProject.Entities;
using BackEndProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly AppDbContext _context;
        public SubscribeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(SubscribeIndexVM model)
        {
            if (ModelState.IsValid)
            {
                var emailModel = new SubscribeEmail
                {
                    Email = model.Email
                };

                _context.Emails.Add(emailModel);
                _context.SaveChanges();

                return View("Subscribe/Index", model);
            }

            return View("Subscribe/Index", model);
        }
    }
}
