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
        public DbSet<ShopSettings> ShopSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial instruments with explicit root-relative image paths
            modelBuilder.Entity<SurgicalInstrument>().HasData(
                new SurgicalInstrument
                {
                    Id = 1,
                    Name = "Scalpels & Blades",
                    Category = "Scalpels",
                    MaterialGrade = "Grade-A Stainless Steel",
                    UnitPrice = 25.00m,
                    StockQuantity = 12400,
                    ImageUrl = "/images/scalpels-blades.jpg"
                },
                new SurgicalInstrument
                {
                    Id = 2,
                    Name = "Hemostatic Forceps",
                    Category = "Forceps",
                    MaterialGrade = "Titanium Coated",
                    UnitPrice = 45.50m,
                    StockQuantity = 8150,
                    ImageUrl = "/images/hemostatic-forceps.jpg"
                },
                new SurgicalInstrument
                {
                    Id = 3,
                    Name = "Retractors & Scissors",
                    Category = "Scissors",
                    MaterialGrade = "Surgical Grade",
                    UnitPrice = 30.00m,
                    StockQuantity = 5200,
                    ImageUrl = "/images/retractors-scissors.jpg"
                }
            );

            // Seed initial orders using DateTime.UtcNow to prevent Npgsql timestamp errors
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 9821,
                    HospitalName = "City Hospital Care",
                    TotalAmount = 14250.00m,
                    Status = "Shipped",
                    OrderDate = DateTime.SpecifyKind(new DateTime(2026, 9, 30), DateTimeKind.Utc)
                },
                new Order
                {
                    Id = 9822,
                    HospitalName = "St. Jude Clinic",
                    TotalAmount = 8900.00m,
                    Status = "Processing",
                    OrderDate = DateTime.SpecifyKind(new DateTime(2026, 10, 1), DateTimeKind.Utc)
                }
            );
        }
    }
}