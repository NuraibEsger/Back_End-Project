using BackEndProject.Areas.Admin.Models;
using BackEndProject.Data;
using BackEndProject.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BackEndProject.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using SendGrid.Helpers.Mail;
using SendGrid;

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
        private async Task SendEmailConfirmationAsync(string email, string subject, string htmlMessage)
        {
            // Use your preferred email service (e.g., SendGrid, SmtpClient)
            // Example using SendGrid:
            var apiKey = "3c685d75a7d841e69a371eaf5b521f9d";
            var emailToValidate = "nurayib.esger@gmail.com";
            var url = "https://emailvalidation.abstractapi.com/v1/?api_key=3c685d75a7d841e69a371eaf5b521f9d&email=nurayib.esger@gmail.com";

            // Use an HTTP client to make the request to the Abstract API

            var client = new SendGridClient(url);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("nurayib.esger@gmail.com", "Your Name"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));

            await client.SendEmailAsync(msg);
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

            // Send email with verification link
            var callbackUrl = Url.Action("Verify", "Authentication", new { token = newUser.VerificationToken }, protocol: HttpContext.Request.Scheme);
            // Implement your email sending logic here, for example using SendGrid or SmtpClient
            // Example using SendGrid:
            await SendEmailConfirmationAsync(newUser.Email, "Confirm your email", $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

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
