using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgiTech.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9821,
                column: "OrderDate",
                value: new DateTime(2026, 9, 15, 9, 48, 9, 992, DateTimeKind.Local).AddTicks(4243));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9822,
                column: "OrderDate",
                value: new DateTime(2026, 9, 16, 9, 48, 9, 992, DateTimeKind.Local).AddTicks(5113));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

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
    }
}
