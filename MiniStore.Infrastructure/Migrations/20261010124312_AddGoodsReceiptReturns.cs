using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceiptReturns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchasePriceVarianceAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GoodsReceiptLines_Id_TenantId",
                table: "GoodsReceiptLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GoodsReceiptId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptReturns", x => x.Id);
                    table.UniqueConstraint("AK_GoodsReceiptReturns_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturns_GoodsReceipts_GoodsReceiptId_TenantId",
                        columns: x => new { x.GoodsReceiptId, x.TenantId },
                        principalTable: "GoodsReceipts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturns_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturns_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturns_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptReturnLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptReturnId = table.Column<int>(type: "int", nullable: false),
                    GoodsReceiptLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ReturnQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    OriginalUnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    OriginalInventoryValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemovedInventoryCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrackingAllocations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptReturnLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturnLines_GoodsReceiptLines_GoodsReceiptLineId_TenantId",
                        columns: x => new { x.GoodsReceiptLineId, x.TenantId },
                        principalTable: "GoodsReceiptLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturnLines_GoodsReceiptReturns_GoodsReceiptReturnId_TenantId",
                        columns: x => new { x.GoodsReceiptReturnId, x.TenantId },
                        principalTable: "GoodsReceiptReturns",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturnLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptReturnLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_PurchasePriceVarianceAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "PurchasePriceVarianceAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturnLines_GoodsReceiptLineId_TenantId",
                table: "GoodsReceiptReturnLines",
                columns: new[] { "GoodsReceiptLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturnLines_GoodsReceiptReturnId_TenantId",
                table: "GoodsReceiptReturnLines",
                columns: new[] { "GoodsReceiptReturnId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturnLines_ProductId_TenantId",
                table: "GoodsReceiptReturnLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturnLines_TenantId",
                table: "GoodsReceiptReturnLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturnLines_TenantId_GoodsReceiptReturnId_GoodsReceiptLineId",
                table: "GoodsReceiptReturnLines",
                columns: new[] { "TenantId", "GoodsReceiptReturnId", "GoodsReceiptLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturns_GoodsReceiptId_TenantId",
                table: "GoodsReceiptReturns",
                columns: new[] { "GoodsReceiptId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturns_PurchaseOrderId_TenantId",
                table: "GoodsReceiptReturns",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturns_SupplierId_TenantId",
                table: "GoodsReceiptReturns",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturns_TenantId",
                table: "GoodsReceiptReturns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturns_TenantId_ReturnNumber",
                table: "GoodsReceiptReturns",
                columns: new[] { "TenantId", "ReturnNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptReturns_WarehouseId_TenantId",
                table: "GoodsReceiptReturns",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_PurchasePriceVarianceAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "PurchasePriceVarianceAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_PurchasePriceVarianceAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropTable(
                name: "GoodsReceiptReturnLines");

            migrationBuilder.DropTable(
                name: "GoodsReceiptReturns");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GoodsReceiptLines_Id_TenantId",
                table: "GoodsReceiptLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_PurchasePriceVarianceAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "PurchasePriceVarianceAccountId",
                table: "AccountingSettings");
        }
    }
}
