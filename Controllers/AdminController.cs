using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SurgiTech.Data;
using SurgiTech.Models;

namespace SurgiTech.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly string[] AllowedStatuses = { "Processing", "Shipped", "Delivered" };

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalStockCount = await _context.Instruments.SumAsync(i => i.StockQuantity),
                TotalCategories = await _context.Instruments.Select(i => i.Category).Distinct().CountAsync(),
                TopInstruments = await _context.Instruments.OrderByDescending(i => i.StockQuantity).Take(5).ToListAsync(),
                RecentOrders = await _context.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToListAsync()
            };

            ViewBag.DisplayName = _context.AdminUsers.FirstOrDefault()?.DisplayName ?? "Admin";

            return View(viewModel);
        }

        // GET: /Admin/Inventory?search=scalpels
        public async Task<IActionResult> Inventory(string? search)
        {
            var query = _context.Instruments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(i =>
                    i.Name.ToLower().Contains(term) ||
                    i.Category.ToLower().Contains(term) ||
                    i.MaterialGrade.ToLower().Contains(term));
            }

            var instruments = await query
                .OrderBy(i => i.Category)
                .ThenBy(i => i.Name)
                .ToListAsync();

            ViewBag.SearchTerm = search;
            return View(instruments);
        }

        // GET: /Admin/AddInstrument
        public IActionResult AddInstrument()
        {
            return View();
        }

        // POST: /Admin/AddInstrument
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInstrument(SurgicalInstrument model, IFormFile? photo)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (photo != null && photo.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("photo", "Only JPG, PNG, or WEBP images are allowed.");
                    return View(model);
                }

                if (photo.Length > 5 * 1024 * 1024) // 5 MB limit
                {
                    ModelState.AddModelError("photo", "Image must be under 5 MB.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "instruments");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                model.ImageUrl = $"/images/instruments/{fileName}";
            }

            _context.Instruments.Add(model);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Added {model.Name}";
            return RedirectToAction(nameof(Inventory));
        }

        // GET: /Admin/EditInstrument/3
        public async Task<IActionResult> EditInstrument(int id)
        {
            var instrument = await _context.Instruments.FindAsync(id);
            if (instrument == null) return NotFound();

            return View(instrument);
        }

        // POST: /Admin/EditInstrument
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInstrument(SurgicalInstrument model, IFormFile? photo)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var instrument = await _context.Instruments.FindAsync(model.Id);
            if (instrument == null) return NotFound();

            instrument.Name = model.Name;
            instrument.Category = model.Category;
            instrument.MaterialGrade = model.MaterialGrade;
            instrument.UnitPrice = model.UnitPrice;
            instrument.StockQuantity = model.StockQuantity;

            if (photo != null && photo.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("photo", "Only JPG, PNG, or WEBP images are allowed.");
                    return View(model);
                }

                if (photo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("photo", "Image must be under 5 MB.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "instruments");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                instrument.ImageUrl = $"/images/instruments/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                instrument.ImageUrl = model.ImageUrl;
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = $"Updated {instrument.Name}";
            return RedirectToAction(nameof(Inventory));
        }

        // GET: /Admin/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.AllowedStatuses = AllowedStatuses;
            return View(orders);
        }

        // POST: /Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            if (!AllowedStatuses.Contains(status))
            {
                TempData["Message"] = "Invalid status.";
                return RedirectToAction(nameof(Orders));
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Order #{order.Id} marked {status}";
            return RedirectToAction(nameof(Orders));
        }

        // GET: /Admin/Reports
        public async Task<IActionResult> Reports()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var summary = AllowedStatuses.Select(status => new StatusSummary
            {
                Status = status,
                Count = orders.Count(o => o.Status == status),
                Total = orders.Where(o => o.Status == status).Sum(o => o.TotalAmount)
            }).ToList();

            var viewModel = new OrderReportViewModel
            {
                Summary = summary,
                Orders = orders,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount)
            };

            return View(viewModel);
        }

        // GET: /Admin/ExportOrdersCsv
        public async Task<IActionResult> ExportOrdersCsv()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Order Number,Hospital/Clinic,Placed,Total Amount,Status");

            foreach (var o in orders)
            {
                var hospitalName = o.HospitalName.Replace("\"", "\"\"");
                csv.AppendLine($"#ORD-{o.Id},\"{hospitalName}\",{o.OrderDate:yyyy-MM-dd},{o.TotalAmount},{o.Status}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"surgitech-orders-{DateTime.Now:yyyy-MM-dd}.csv";
            return File(bytes, "text/csv", fileName);
        }

        // GET: /Admin/Settings
        public async Task<IActionResult> Settings()
        {
            var admin = await _context.AdminUsers.FirstOrDefaultAsync();
            var shopSettings = await GetOrCreateShopSettingsAsync();

            var viewModel = new SettingsViewModel
            {
                DisplayName = admin?.DisplayName ?? "",
                Email = admin?.Email ?? "",
                WhatsAppNumber = shopSettings.WhatsAppNumber,
                ContactEmail = shopSettings.ContactEmail,
                ShippingFee = shopSettings.ShippingFee
            };

            return View(viewModel);
        }

        // POST: /Admin/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string displayName, string email)
        {
            var admin = await _context.AdminUsers.FirstOrDefaultAsync();
            if (admin == null) return RedirectToAction(nameof(Settings));

            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(email))
            {
                TempData["ProfileError"] = "Name and email can't be empty.";
                return RedirectToAction(nameof(Settings));
            }

            admin.DisplayName = displayName.Trim();
            admin.Email = email.Trim();
            admin.Username = email.Trim();

            await _context.SaveChangesAsync();
            TempData["Message"] = "Profile updated.";
            return RedirectToAction(nameof(Settings));
        }

        // POST: /Admin/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var admin = await _context.AdminUsers.FirstOrDefaultAsync();
            if (admin == null) return RedirectToAction(nameof(Settings));

            if (admin.Password != currentPassword)
            {
                TempData["PasswordError"] = "Current password is incorrect.";
                return RedirectToAction(nameof(Settings));
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
            {
                TempData["PasswordError"] = "New password must be at least 4 characters.";
                return RedirectToAction(nameof(Settings));
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordError"] = "New password and confirmation don't match.";
                return RedirectToAction(nameof(Settings));
            }

            admin.Password = newPassword;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password changed.";
            return RedirectToAction(nameof(Settings));
        }

        // POST: /Admin/UpdateShopSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShopSettings(string whatsAppNumber, string contactEmail, decimal shippingFee)
        {
            var shopSettings = await GetOrCreateShopSettingsAsync();

            shopSettings.WhatsAppNumber = (whatsAppNumber ?? "").Trim();
            shopSettings.ContactEmail = (contactEmail ?? "").Trim();
            shopSettings.ShippingFee = shippingFee < 0 ? 0 : shippingFee;

            await _context.SaveChangesAsync();
            TempData["Message"] = "Shop settings updated.";
            return RedirectToAction(nameof(Settings));
        }

        private async Task<ShopSettings> GetOrCreateShopSettingsAsync()
        {
            var settings = await _context.ShopSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new ShopSettings();
                _context.ShopSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings;
        }
    }

    public class StatusSummary
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderReportViewModel
    {
        public List<StatusSummary> Summary { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SettingsViewModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public decimal ShippingFee { get; set; }
    }
}