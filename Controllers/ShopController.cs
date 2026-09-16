using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurgiTech.Data;
using SurgiTech.Extensions;
using SurgiTech.Models;

namespace SurgiTech.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Shop/Index (Client Product Catalog)
        public async Task<IActionResult> Index(string? category)
        {
            var query = _context.Instruments.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(i => i.Category == category);
            }

            var products = await query.ToListAsync();

            ViewBag.Categories = await _context.Instruments
                .Select(i => i.Category)
                .Distinct()
                .ToListAsync();
            ViewBag.SelectedCategory = category;
            ViewBag.CartCount = GetCart().Sum(c => c.Quantity);

            return View(products);
        }

        // GET: /Shop/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Instruments.FirstOrDefaultAsync(i => i.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: /Shop/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _context.Instruments.FindAsync(id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.InstrumentId == id);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    InstrumentId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            TempData["Message"] = $"Added {product.Name} to cart";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Shop/Cart
        public IActionResult Cart()
        {
            return View(GetCart());
        }

        // POST: /Shop/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.InstrumentId == id);
            if (item != null)
            {
                item.Quantity = Math.Max(1, quantity);
            }
            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        // POST: /Shop/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.InstrumentId == id);
            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        // POST: /Shop/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string hospitalName)
        {
            var cart = GetCart();

            if (string.IsNullOrWhiteSpace(hospitalName))
            {
                TempData["Message"] = "Enter your hospital or clinic name to place the order.";
                return RedirectToAction(nameof(Cart));
            }

            if (!cart.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction(nameof(Cart));
            }

            var order = new Order
            {
                HospitalName = hospitalName.Trim(),
                TotalAmount = cart.Sum(c => c.LineTotal),
                Status = "Processing",
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            SaveCart(new List<CartItem>());

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        // GET: /Shop/Confirmation/9823
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: /Shop/MyOrders?hospitalName=City Hospital Care
        public async Task<IActionResult> MyOrders(string? hospitalName)
        {
            var orders = new List<Order>();

            if (!string.IsNullOrWhiteSpace(hospitalName))
            {
                orders = await _context.Orders
                    .Where(o => o.HospitalName.ToLower() == hospitalName.Trim().ToLower())
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            ViewBag.HospitalName = hospitalName;
            return View(orders);
        }

        // GET: /Shop/Track?id=9821
        public async Task<IActionResult> Track(int? id)
        {
            Order? order = null;

            if (id.HasValue)
            {
                order = await _context.Orders.FindAsync(id.Value);
                if (order == null)
                {
                    ViewBag.NotFoundId = id.Value;
                }
            }

            return View(order);
        }

        private List<CartItem> GetCart()
        {
            return HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObject(CartSessionKey, cart);
        }
    }
}