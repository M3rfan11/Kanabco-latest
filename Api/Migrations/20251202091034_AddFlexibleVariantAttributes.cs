using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFlexibleVariantAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attributes",
                table: "ProductVariants",
                type: "TEXT",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantAttributes",
                table: "Products",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 9, 10, 33, 593, DateTimeKind.Utc).AddTicks(1840));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 9, 10, 33, 593, DateTimeKind.Utc).AddTicks(3300));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 9, 10, 33, 593, DateTimeKind.Utc).AddTicks(3300));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 9, 10, 33, 593, DateTimeKind.Utc).AddTicks(3300));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 9, 10, 33, 593, DateTimeKind.Utc).AddTicks(3300));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 9, 10, 33, 593, DateTimeKind.Utc).AddTicks(3310));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attributes",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "VariantAttributes",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 1, 13, 26, 17, 87, DateTimeKind.Utc).AddTicks(5920));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 1, 13, 26, 17, 87, DateTimeKind.Utc).AddTicks(9320));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 1, 13, 26, 17, 87, DateTimeKind.Utc).AddTicks(9320));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 1, 13, 26, 17, 87, DateTimeKind.Utc).AddTicks(9320));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 1, 13, 26, 17, 87, DateTimeKind.Utc).AddTicks(9330));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 1, 13, 26, 17, 87, DateTimeKind.Utc).AddTicks(9330));
        }
    }
}
