using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierCreditNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierCreditNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditNoteNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorBillId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    CreditDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SupplierReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_SupplierCreditNotes", x => x.Id);
                    table.UniqueConstraint("AK_SupplierCreditNotes_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SupplierCreditNotes_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNotes_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNotes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNotes_VendorBills_VendorBillId_TenantId",
                        columns: x => new { x.VendorBillId, x.TenantId },
                        principalTable: "VendorBills",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNotes_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierCreditNoteLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierCreditNoteId = table.Column<int>(type: "int", nullable: false),
                    VendorBillLineId = table.Column<int>(type: "int", nullable: false),
                    GoodsReceiptLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TaxInputAccountId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemovedInventoryCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrackingAllocations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCreditNoteLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNoteLines_Accounts_TaxInputAccountId_TenantId",
                        columns: x => new { x.TaxInputAccountId, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNoteLines_GoodsReceiptLines_GoodsReceiptLineId_TenantId",
                        columns: x => new { x.GoodsReceiptLineId, x.TenantId },
                        principalTable: "GoodsReceiptLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNoteLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNoteLines_SupplierCreditNotes_SupplierCreditNoteId_TenantId",
                        columns: x => new { x.SupplierCreditNoteId, x.TenantId },
                        principalTable: "SupplierCreditNotes",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNoteLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditNoteLines_VendorBillLines_VendorBillLineId_TenantId",
                        columns: x => new { x.VendorBillLineId, x.TenantId },
                        principalTable: "VendorBillLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_GoodsReceiptLineId_TenantId",
                table: "SupplierCreditNoteLines",
                columns: new[] { "GoodsReceiptLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_ProductId_TenantId",
                table: "SupplierCreditNoteLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_SupplierCreditNoteId_TenantId",
                table: "SupplierCreditNoteLines",
                columns: new[] { "SupplierCreditNoteId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_TaxInputAccountId_TenantId",
                table: "SupplierCreditNoteLines",
                columns: new[] { "TaxInputAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_TenantId",
                table: "SupplierCreditNoteLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_TenantId_SupplierCreditNoteId_VendorBillLineId",
                table: "SupplierCreditNoteLines",
                columns: new[] { "TenantId", "SupplierCreditNoteId", "VendorBillLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNoteLines_VendorBillLineId_TenantId",
                table: "SupplierCreditNoteLines",
                columns: new[] { "VendorBillLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNotes_PurchaseOrderId_TenantId",
                table: "SupplierCreditNotes",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNotes_SupplierId_TenantId",
                table: "SupplierCreditNotes",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNotes_TenantId",
                table: "SupplierCreditNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNotes_TenantId_CreditNoteNumber",
                table: "SupplierCreditNotes",
                columns: new[] { "TenantId", "CreditNoteNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNotes_VendorBillId_TenantId",
                table: "SupplierCreditNotes",
                columns: new[] { "VendorBillId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditNotes_WarehouseId_TenantId",
                table: "SupplierCreditNotes",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierCreditNoteLines");

            migrationBuilder.DropTable(
                name: "SupplierCreditNotes");
        }
    }
}
