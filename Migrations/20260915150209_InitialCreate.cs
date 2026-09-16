using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurgiTech.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialGrade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Instruments",
                columns: new[] { "Id", "Category", "ImageUrl", "MaterialGrade", "Name", "StockQuantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Scalpels", "/images/default-instrument.jpg", "Grade-A Stainless Steel", "Scalpels & Blades", 12400, 25.00m },
                    { 2, "Forceps", "/images/default-instrument.jpg", "Titanium Coated", "Hemostatic Forceps", 8150, 45.50m },
                    { 3, "Scissors", "/images/default-instrument.jpg", "Surgical Grade", "Retractors & Scissors", 5200, 30.00m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "HospitalName", "OrderDate", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 9821, "City Hospital Care", new DateTime(2026, 9, 14, 20, 2, 6, 752, DateTimeKind.Local).AddTicks(4065), "Shipped", 14250.00m },
                    { 9822, "St. Jude Clinic", new DateTime(2026, 9, 15, 20, 2, 6, 752, DateTimeKind.Local).AddTicks(4926), "Processing", 8900.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
