using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistTableOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodes_Users_CreatedByUserId",
                table: "PromoCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                table: "PromoCodeUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodeUsages_SalesOrders_SalesOrderId",
                table: "PromoCodeUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodeUsages_Users_UserId",
                table: "PromoCodeUsages");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodeUsers_PromoCodeId",
                table: "PromoCodeUsers");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            // IsNotified and NotifiedAt columns already exist in PromoCodeUsers table
            // Skipping these operations to avoid duplicate column error

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            // UsageLimitPerUser column already exists in PromoCodes table
            // Skipping this operation to avoid duplicate column error

            // PromoCodeProducts table already exists
            // Skipping table creation to avoid duplicate table error

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductVariantId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 11, 16, 59, 32, 595, DateTimeKind.Utc).AddTicks(7890));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 11, 16, 59, 32, 595, DateTimeKind.Utc).AddTicks(9620));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 11, 16, 59, 32, 595, DateTimeKind.Utc).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 11, 16, 59, 32, 595, DateTimeKind.Utc).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 11, 16, 59, 32, 595, DateTimeKind.Utc).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 11, 16, 59, 32, 595, DateTimeKind.Utc).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Phone" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsers_PromoCodeId_UserId",
                table: "PromoCodeUsers",
                columns: new[] { "PromoCodeId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeProducts_ProductId",
                table: "PromoCodeProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeProducts_PromoCodeId_ProductId",
                table: "PromoCodeProducts",
                columns: new[] { "PromoCodeId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ProductId",
                table: "Wishlists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ProductVariantId",
                table: "Wishlists",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId_ProductId_ProductVariantId",
                table: "Wishlists",
                columns: new[] { "UserId", "ProductId", "ProductVariantId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodes_Users_CreatedByUserId",
                table: "PromoCodes",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                table: "PromoCodeUsages",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodeUsages_SalesOrders_SalesOrderId",
                table: "PromoCodeUsages",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodeUsages_Users_UserId",
                table: "PromoCodeUsages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodes_Users_CreatedByUserId",
                table: "PromoCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                table: "PromoCodeUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodeUsages_SalesOrders_SalesOrderId",
                table: "PromoCodeUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodeUsages_Users_UserId",
                table: "PromoCodeUsages");

            // PromoCodeProducts table should not be dropped as it exists
            // Skipping this operation

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodeUsers_PromoCodeId_UserId",
                table: "PromoCodeUsers");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            // IsNotified and NotifiedAt columns should not be dropped as they exist
            // Skipping these operations

            // UsageLimitPerUser column should not be dropped as it exists
            // Skipping this operation

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 14, 25, 1, 888, DateTimeKind.Utc).AddTicks(5680));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 14, 25, 1, 888, DateTimeKind.Utc).AddTicks(7500));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 14, 25, 1, 888, DateTimeKind.Utc).AddTicks(7510));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 14, 25, 1, 888, DateTimeKind.Utc).AddTicks(7510));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 14, 25, 1, 888, DateTimeKind.Utc).AddTicks(7510));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 14, 25, 1, 888, DateTimeKind.Utc).AddTicks(7510));

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsers_PromoCodeId",
                table: "PromoCodeUsers",
                column: "PromoCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodes_Users_CreatedByUserId",
                table: "PromoCodes",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                table: "PromoCodeUsages",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodeUsages_SalesOrders_SalesOrderId",
                table: "PromoCodeUsages",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodeUsages_Users_UserId",
                table: "PromoCodeUsages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
