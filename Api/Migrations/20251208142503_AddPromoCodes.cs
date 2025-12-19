using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DiscountType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsageLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    UsedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaximumDiscountAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PromoCodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalesOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PromoCodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsers_PromoCodes_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsers_Users_UserId",
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
                name: "IX_PromoCodes_CreatedByUserId",
                table: "PromoCodes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_PromoCodeId",
                table: "PromoCodeUsages",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_SalesOrderId",
                table: "PromoCodeUsages",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_UserId",
                table: "PromoCodeUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsers_PromoCodeId",
                table: "PromoCodeUsers",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsers_UserId",
                table: "PromoCodeUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoCodeUsages");

            migrationBuilder.DropTable(
                name: "PromoCodeUsers");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 14, 42, 9, 869, DateTimeKind.Utc).AddTicks(3320));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 14, 42, 9, 869, DateTimeKind.Utc).AddTicks(5530));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 14, 42, 9, 869, DateTimeKind.Utc).AddTicks(5530));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 14, 42, 9, 869, DateTimeKind.Utc).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 14, 42, 9, 869, DateTimeKind.Utc).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 14, 42, 9, 869, DateTimeKind.Utc).AddTicks(5550));
        }
    }
}
