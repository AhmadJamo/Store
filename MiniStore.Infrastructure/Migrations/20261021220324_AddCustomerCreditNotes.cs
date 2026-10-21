using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCreditNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_CustomerReturnReceiptLines_Id_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "CustomerCreditNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditNoteNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerInvoiceId = table.Column<int>(type: "int", nullable: false),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreditDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReceivableAccountIdSnapshot = table.Column<int>(type: "int", nullable: false),
                    SalesRevenueAccountIdSnapshot = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CustomerCreditNotes", x => x.Id);
                    table.UniqueConstraint("AK_CustomerCreditNotes_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_CustomerCreditNotes_Accounts_ReceivableAccountIdSnapshot_TenantId",
                        columns: x => new { x.ReceivableAccountIdSnapshot, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNotes_Accounts_SalesRevenueAccountIdSnapshot_TenantId",
                        columns: x => new { x.SalesRevenueAccountIdSnapshot, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNotes_CustomerInvoices_CustomerInvoiceId_TenantId",
                        columns: x => new { x.CustomerInvoiceId, x.TenantId },
                        principalTable: "CustomerInvoices",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNotes_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNotes_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNotes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCreditNoteLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCreditNoteId = table.Column<int>(type: "int", nullable: false),
                    CustomerInvoiceLineId = table.Column<int>(type: "int", nullable: false),
                    CustomerReturnReceiptLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    TaxOutputAccountId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompanyNetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompanyTaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompanyGrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCreditNoteLines", x => x.Id);
                    table.CheckConstraint("CK_CustomerCreditNoteLines_Quantity", "[Quantity] > 0 AND [StockQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_Accounts_TaxOutputAccountId_TenantId",
                        columns: x => new { x.TaxOutputAccountId, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_CustomerCreditNotes_CustomerCreditNoteId_TenantId",
                        columns: x => new { x.CustomerCreditNoteId, x.TenantId },
                        principalTable: "CustomerCreditNotes",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_CustomerInvoiceLines_CustomerInvoiceLineId_TenantId",
                        columns: x => new { x.CustomerInvoiceLineId, x.TenantId },
                        principalTable: "CustomerInvoiceLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_CustomerReturnReceiptLines_CustomerReturnReceiptLineId_TenantId",
                        columns: x => new { x.CustomerReturnReceiptLineId, x.TenantId },
                        principalTable: "CustomerReturnReceiptLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCreditNoteLines_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_CustomerCreditNoteId_TenantId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "CustomerCreditNoteId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_CustomerInvoiceLineId_TenantId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "CustomerInvoiceLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_CustomerReturnReceiptLineId_TenantId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "CustomerReturnReceiptLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_ProductId_TenantId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_TaxOutputAccountId_TenantId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "TaxOutputAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_TenantId",
                table: "CustomerCreditNoteLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_TenantId_CustomerCreditNoteId_CustomerInvoiceLineId_CustomerReturnReceiptLineId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "TenantId", "CustomerCreditNoteId", "CustomerInvoiceLineId", "CustomerReturnReceiptLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNoteLines_WarehouseId_TenantId",
                table: "CustomerCreditNoteLines",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_CustomerId_TenantId",
                table: "CustomerCreditNotes",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_CustomerInvoiceId_Status",
                table: "CustomerCreditNotes",
                columns: new[] { "CustomerInvoiceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_CustomerInvoiceId_TenantId",
                table: "CustomerCreditNotes",
                columns: new[] { "CustomerInvoiceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_ReceivableAccountIdSnapshot_TenantId",
                table: "CustomerCreditNotes",
                columns: new[] { "ReceivableAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_SalesOrderId_TenantId",
                table: "CustomerCreditNotes",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_SalesRevenueAccountIdSnapshot_TenantId",
                table: "CustomerCreditNotes",
                columns: new[] { "SalesRevenueAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_TenantId",
                table: "CustomerCreditNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_TenantId_CreditNoteNumber",
                table: "CustomerCreditNotes",
                columns: new[] { "TenantId", "CreditNoteNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCreditNotes_TenantId_RequestKey",
                table: "CustomerCreditNotes",
                columns: new[] { "TenantId", "RequestKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerCreditNoteLines");

            migrationBuilder.DropTable(
                name: "CustomerCreditNotes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CustomerReturnReceiptLines_Id_TenantId",
                table: "CustomerReturnReceiptLines");
        }
    }
}
