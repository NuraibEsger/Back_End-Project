using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BackEndProject.Services;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;


namespace BackEndProject.Areas.Admin.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly SendEmail _sendEmail;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AuthenticationController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            AppDbContext dbContext, 
            IWebHostEnvironment hostingEnvironment,
            SendEmail sendEmail)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
            _sendEmail = sendEmail;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return BadRequest();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthenticationLoginVM model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("", "You must have a confirmed email to log on.");
                    return View();
                }
            }
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
        public async Task<ActionResult> Register(AuthenticationRegisterVM model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user is not null) return RedirectToAction("Login","Authentication");

            var newUser = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var confirmationLink = Url.Action("ConfirmEmail", "Authentication", new { token, email = model.Email }, Request.Scheme);

                bool IsSendEmail = await _sendEmail.EmailSend(model.Email!,confirmationLink);
                if (IsSendEmail)
                {
                    return RedirectToAction("Login", "Authentication");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmEmail", "Auhtentication");
            }
            return RedirectToAction("View");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Authentication");
        }
    }
}
