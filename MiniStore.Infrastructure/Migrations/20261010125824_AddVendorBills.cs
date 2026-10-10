using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorBills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SupplierInvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NormalizedSupplierInvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_VendorBills", x => x.Id);
                    table.UniqueConstraint("AK_VendorBills_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_VendorBills_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBills_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBills_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorBillLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorBillId = table.Column<int>(type: "int", nullable: false),
                    GoodsReceiptLineId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TaxRateId = table.Column<int>(type: "int", nullable: true),
                    TaxInputAccountId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReceiptUnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    IsTaxInclusive = table.Column<bool>(type: "bit", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceiptClearingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBillLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_Accounts_TaxInputAccountId_TenantId",
                        columns: x => new { x.TaxInputAccountId, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_GoodsReceiptLines_GoodsReceiptLineId_TenantId",
                        columns: x => new { x.GoodsReceiptLineId, x.TenantId },
                        principalTable: "GoodsReceiptLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_PurchaseOrderLines_PurchaseOrderLineId_TenantId",
                        columns: x => new { x.PurchaseOrderLineId, x.TenantId },
                        principalTable: "PurchaseOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_TaxRates_TaxRateId_TenantId",
                        columns: x => new { x.TaxRateId, x.TenantId },
                        principalTable: "TaxRates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillLines_VendorBills_VendorBillId_TenantId",
                        columns: x => new { x.VendorBillId, x.TenantId },
                        principalTable: "VendorBills",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_GoodsReceiptLineId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "GoodsReceiptLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_ProductId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_PurchaseOrderLineId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "PurchaseOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TaxInputAccountId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "TaxInputAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TaxRateId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TenantId",
                table: "VendorBillLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_GoodsReceiptLineId",
                table: "VendorBillLines",
                columns: new[] { "TenantId", "VendorBillId", "GoodsReceiptLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_VendorBillId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "VendorBillId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBills_PurchaseOrderId_TenantId",
                table: "VendorBills",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBills_SupplierId_TenantId",
                table: "VendorBills",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBills_TenantId",
                table: "VendorBills",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBills_TenantId_BillNumber",
                table: "VendorBills",
                columns: new[] { "TenantId", "BillNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorBills_TenantId_SupplierId_NormalizedSupplierInvoiceNumber",
                table: "VendorBills",
                columns: new[] { "TenantId", "SupplierId", "NormalizedSupplierInvoiceNumber" },
                unique: true,
                filter: "[Status] <> 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorBillLines");

            migrationBuilder.DropTable(
                name: "VendorBills");
        }
    }
}
