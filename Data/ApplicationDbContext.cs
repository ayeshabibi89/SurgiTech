using Microsoft.EntityFrameworkCore;
using SurgiTech.Models;

namespace SurgiTech.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AdminUser> AdminUsers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<SurgicalInstrument> Instruments { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial data
            modelBuilder.Entity<SurgicalInstrument>().HasData(
                new SurgicalInstrument { Id = 1, Name = "Scalpels & Blades", Category = "Scalpels", MaterialGrade = "Grade-A Stainless Steel", UnitPrice = 25.00m, StockQuantity = 12400 },
                new SurgicalInstrument { Id = 2, Name = "Hemostatic Forceps", Category = "Forceps", MaterialGrade = "Titanium Coated", UnitPrice = 45.50m, StockQuantity = 8150 },
                new SurgicalInstrument { Id = 3, Name = "Retractors & Scissors", Category = "Scissors", MaterialGrade = "Surgical Grade", UnitPrice = 30.00m, StockQuantity = 5200 }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 9821, HospitalName = "City Hospital Care", TotalAmount = 14250.00m, Status = "Shipped", OrderDate = DateTime.Now.AddDays(-1) },
                new Order { Id = 9822, HospitalName = "St. Jude Clinic", TotalAmount = 8900.00m, Status = "Processing", OrderDate = DateTime.Now }
            );
        }
    }
}