using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgiTech.Migrations
{
    /// <inheritdoc />
    public partial class AddCartSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9821,
                column: "OrderDate",
                value: new DateTime(2026, 9, 15, 8, 55, 20, 75, DateTimeKind.Local).AddTicks(5428));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9822,
                column: "OrderDate",
                value: new DateTime(2026, 9, 16, 8, 55, 20, 100, DateTimeKind.Local).AddTicks(2679));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9821,
                column: "OrderDate",
                value: new DateTime(2026, 9, 14, 20, 2, 6, 752, DateTimeKind.Local).AddTicks(4065));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9822,
                column: "OrderDate",
                value: new DateTime(2026, 9, 15, 20, 2, 6, 752, DateTimeKind.Local).AddTicks(4926));
        }
    }
}
