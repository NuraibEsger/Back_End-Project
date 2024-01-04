using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BackEndProject.Areas.Admin.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext? _dbContext;

        public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return BadRequest();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AuthenticationLoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, false, false);

            if (!result.Succeeded) return View(model);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return BadRequest();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AuthenticationRegisterVM model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user is not null) BadRequest();

            var newUser = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                VerificationToken = CreateRandomToken()
            };

            var result = await _userManager.CreateAsync(newUser, model.Password!);
            if (!result.Succeeded) return View();

            return RedirectToAction(nameof(Login));
        }
        [HttpPost("Verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x=> x.VerificationToken == token);
            if(user is null) return BadRequest("Invalid Token");

            await _dbContext.SaveChangesAsync();

            return Ok("User Verified");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Authentication");
        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
