using BackEndProject.Areas.Admin.Models;
using BackEndProject.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.Admin.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(newUser, model.Password!);
            if (!result.Succeeded) return View();

            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Authentication");
        }
    }
}
