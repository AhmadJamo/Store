using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierPriceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceHistoryRevision",
                table: "SupplierProductPurchasingInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupplierProductPriceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierProductPurchasingInfoId = table.Column<int>(type: "int", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PurchaseMeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    MinimumOrderQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    BaseUnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProductPriceHistories", x => x.Id);
                    table.UniqueConstraint("AK_SupplierProductPriceHistories_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceHistories_MeasurementUnits_PurchaseMeasurementUnitId_TenantId",
                        columns: x => new { x.PurchaseMeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceHistories_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceHistories_SupplierProductPurchasingInfos_SupplierProductPurchasingInfoId_TenantId",
                        columns: x => new { x.SupplierProductPurchasingInfoId, x.TenantId },
                        principalTable: "SupplierProductPurchasingInfos",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceHistories_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProductPriceBreakSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierProductPriceHistoryId = table.Column<int>(type: "int", nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProductPriceBreakSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceBreakSnapshots_SupplierProductPriceHistories_SupplierProductPriceHistoryId_TenantId",
                        columns: x => new { x.SupplierProductPriceHistoryId, x.TenantId },
                        principalTable: "SupplierProductPriceHistories",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceBreakSnapshots_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceBreakSnapshots_SupplierProductPriceHistoryId_TenantId",
                table: "SupplierProductPriceBreakSnapshots",
                columns: new[] { "SupplierProductPriceHistoryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceBreakSnapshots_TenantId",
                table: "SupplierProductPriceBreakSnapshots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceBreakSnapshots_TenantId_SupplierProductPriceHistoryId_MinimumQuantity",
                table: "SupplierProductPriceBreakSnapshots",
                columns: new[] { "TenantId", "SupplierProductPriceHistoryId", "MinimumQuantity" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_ProductId_TenantId",
                table: "SupplierProductPriceHistories",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_PurchaseMeasurementUnitId_TenantId",
                table: "SupplierProductPriceHistories",
                columns: new[] { "PurchaseMeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_SupplierId_TenantId",
                table: "SupplierProductPriceHistories",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_SupplierProductPurchasingInfoId_ChangedAtUtc",
                table: "SupplierProductPriceHistories",
                columns: new[] { "SupplierProductPurchasingInfoId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_SupplierProductPurchasingInfoId_TenantId",
                table: "SupplierProductPriceHistories",
                columns: new[] { "SupplierProductPurchasingInfoId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_TenantId",
                table: "SupplierProductPriceHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceHistories_TenantId_SupplierProductPurchasingInfoId_Revision",
                table: "SupplierProductPriceHistories",
                columns: new[] { "TenantId", "SupplierProductPurchasingInfoId", "Revision" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierProductPriceBreakSnapshots");

            migrationBuilder.DropTable(
                name: "SupplierProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "PriceHistoryRevision",
                table: "SupplierProductPurchasingInfos");
        }
    }
}
