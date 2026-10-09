using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceiptFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_PurchaseOrderLines_Id_TenantId",
                table: "PurchaseOrderLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "GoodsReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceipts", x => x.Id);
                    table.UniqueConstraint("AK_GoodsReceipts_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LotNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNumbers = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ManufactureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptLines_GoodsReceipts_GoodsReceiptId_TenantId",
                        columns: x => new { x.GoodsReceiptId, x.TenantId },
                        principalTable: "GoodsReceipts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptLines_PurchaseOrderLines_PurchaseOrderLineId_TenantId",
                        columns: x => new { x.PurchaseOrderLineId, x.TenantId },
                        principalTable: "PurchaseOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptLines_GoodsReceiptId_TenantId",
                table: "GoodsReceiptLines",
                columns: new[] { "GoodsReceiptId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptLines_MeasurementUnitId_TenantId",
                table: "GoodsReceiptLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptLines_ProductId_TenantId",
                table: "GoodsReceiptLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptLines_PurchaseOrderLineId_TenantId",
                table: "GoodsReceiptLines",
                columns: new[] { "PurchaseOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptLines_TenantId",
                table: "GoodsReceiptLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptLines_TenantId_GoodsReceiptId_PurchaseOrderLineId",
                table: "GoodsReceiptLines",
                columns: new[] { "TenantId", "GoodsReceiptId", "PurchaseOrderLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_PurchaseOrderId_TenantId",
                table: "GoodsReceipts",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_TenantId",
                table: "GoodsReceipts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_TenantId_ReceiptNumber",
                table: "GoodsReceipts",
                columns: new[] { "TenantId", "ReceiptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_WarehouseId_TenantId",
                table: "GoodsReceipts",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceiptLines");

            migrationBuilder.DropTable(
                name: "GoodsReceipts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PurchaseOrderLines_Id_TenantId",
                table: "PurchaseOrderLines");
        }
    }
}
