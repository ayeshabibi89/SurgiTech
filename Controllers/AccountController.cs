using Microsoft.AspNetCore.Mvc;
using SurgiTech.Data;
using SurgiTech.Models;

namespace SurgiTech.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.IsFirstTime = !_context.AdminUsers.Any();
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, string? displayName)
        {
            var admin = _context.AdminUsers.FirstOrDefault();

            // First time setup: this becomes the one and only admin account
            if (admin == null)
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Enter both a Gmail address and a password.";
                    ViewBag.IsFirstTime = true;
                    return View();
                }

                var newAdmin = new AdminUser
                {
                    Username = email.Trim(),
                    Email = email.Trim(),
                    Password = password,
                    DisplayName = string.IsNullOrWhiteSpace(displayName)
                        ? email.Split('@')[0]
                        : displayName.Trim()
                };

                _context.AdminUsers.Add(newAdmin);
                _context.SaveChanges();

                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard", "Admin");
            }

            // Regular login: check against the saved account
            if (string.Equals(admin.Email, email?.Trim(), StringComparison.OrdinalIgnoreCase)
                && admin.Password == password)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.Error = "Incorrect Gmail address or password.";
            ViewBag.IsFirstTime = false;
            return View();
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Shop");
        }
    }
}