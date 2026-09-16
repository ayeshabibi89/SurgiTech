using System.Collections.Generic;

namespace SurgiTech.Models
{
    public class DashboardViewModel
    {
        public int TotalStockCount { get; set; }
        public int TotalCategories { get; set; }
        public List<SurgicalInstrument> TopInstruments { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }
}