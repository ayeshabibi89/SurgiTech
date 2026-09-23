using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurgiTech.Data;

namespace SurgiTech.Components
{
    public class ShopContactInfoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ShopContactInfoViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settings = await _context.ShopSettings.FirstOrDefaultAsync();

            var model = settings ?? new SurgiTech.Models.ShopSettings
            {
                WhatsAppNumber = "",
                ContactEmail = "orders@surgitech.example"
            };

            return View(model);
        }
    }
}