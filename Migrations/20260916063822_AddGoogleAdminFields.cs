using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgiTech.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleAdminFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "AdminUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AdminUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AdminUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9821,
                column: "OrderDate",
                value: new DateTime(2026, 9, 15, 11, 38, 20, 800, DateTimeKind.Local).AddTicks(1555));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9822,
                column: "OrderDate",
                value: new DateTime(2026, 9, 16, 11, 38, 20, 800, DateTimeKind.Local).AddTicks(2475));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AdminUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "AdminUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
