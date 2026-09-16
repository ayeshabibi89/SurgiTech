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

        // Runs before every action in this controller, so the login
        // check no longer needs to be repeated in each method.
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

        // GET: /Admin/Inventory
        public async Task<IActionResult> Inventory()
        {
            var instruments = await _context.Instruments
                .OrderBy(i => i.Category)
                .ThenBy(i => i.Name)
                .ToListAsync();

            return View(instruments);
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
        public async Task<IActionResult> EditInstrument(SurgicalInstrument model)
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

            if (!string.IsNullOrWhiteSpace(model.ImageUrl))
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
    }
}