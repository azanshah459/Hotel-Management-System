using System.Security.Claims;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<UserAccount> _hasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalized = model.Username.Trim().ToLower();
            var user = await _context.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalized);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var verifyResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.Role == "Admin")
            {
                return RedirectToAction("Admin", "Dashboard");
            }

            return RedirectToAction("UserDashboard", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalized = model.Username.Trim().ToLower();
            var exists = await _context.UserAccounts
                .AnyAsync(u => u.Username.ToLower() == normalized);

            if (exists)
            {
                ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
                return View(model);
            }

            var user = new UserAccount
            {
                Username = model.Username.Trim(),
                Role = "User"
            };

            user.PasswordHash = _hasher.HashPassword(user, model.Password);
            _context.UserAccounts.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account created. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalized = model.Username.Trim().ToLower();
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalized);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Username), "No account found with that username.");
                return View(model);
            }

            user.PasswordHash = _hasher.HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password updated. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            if (username == null) return RedirectToAction("Login");

            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();

            return View(user);
        }

    }
}
