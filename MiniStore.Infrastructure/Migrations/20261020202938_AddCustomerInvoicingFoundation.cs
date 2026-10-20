using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerInvoicingFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalesInvoicePolicy",
                table: "AccountingSettings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SalesDeliveryLines_Id_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "CustomerInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
                    QuantityPolicy = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerLegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerTaxNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerAddressId = table.Column<int>(type: "int", nullable: true),
                    AddressLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StateOrProvince = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    ExchangeRateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExchangeRateSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentTermDays = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoices", x => x.Id);
                    table.UniqueConstraint("AK_CustomerInvoices_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_CustomerInvoices_CustomerAddresses_CustomerAddressId_TenantId",
                        columns: x => new { x.CustomerAddressId, x.TenantId },
                        principalTable: "CustomerAddresses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoices_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoices_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoices_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInvoiceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerInvoiceId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoiceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceHistories_CustomerInvoices_CustomerInvoiceId_TenantId",
                        columns: x => new { x.CustomerInvoiceId, x.TenantId },
                        principalTable: "CustomerInvoices",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInvoiceLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerInvoiceId = table.Column<int>(type: "int", nullable: false),
                    SalesOrderLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitFactorToStock = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    QuantityBasis = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SalesPriceListId = table.Column<int>(type: "int", nullable: true),
                    SalesPriceRuleId = table.Column<int>(type: "int", nullable: true),
                    PriceSourceCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CompanyUnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxRateId = table.Column<int>(type: "int", nullable: true),
                    TaxName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxRatePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    IsTaxInclusive = table.Column<bool>(type: "bit", nullable: false),
                    TaxSourceCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TaxExemptionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoiceLines", x => x.Id);
                    table.UniqueConstraint("AK_CustomerInvoiceLines_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_CustomerInvoiceLines_Quantity", "[Quantity] > 0 AND [StockQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_CustomerInvoices_CustomerInvoiceId_TenantId",
                        columns: x => new { x.CustomerInvoiceId, x.TenantId },
                        principalTable: "CustomerInvoices",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_SalesOrderLines_SalesOrderLineId_TenantId",
                        columns: x => new { x.SalesOrderLineId, x.TenantId },
                        principalTable: "SalesOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_SalesPriceLists_SalesPriceListId_TenantId",
                        columns: x => new { x.SalesPriceListId, x.TenantId },
                        principalTable: "SalesPriceLists",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_SalesPriceRules_SalesPriceRuleId_TenantId",
                        columns: x => new { x.SalesPriceRuleId, x.TenantId },
                        principalTable: "SalesPriceRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_TaxRates_TaxRateId_TenantId",
                        columns: x => new { x.TaxRateId, x.TenantId },
                        principalTable: "TaxRates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInvoiceSourceAllocations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerInvoiceLineId = table.Column<int>(type: "int", nullable: false),
                    SalesDeliveryId = table.Column<int>(type: "int", nullable: false),
                    SalesDeliveryLineId = table.Column<int>(type: "int", nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoiceSourceAllocations", x => x.Id);
                    table.CheckConstraint("CK_CustomerInvoiceSourceAllocations_Quantity", "[StockQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceSourceAllocations_CustomerInvoiceLines_CustomerInvoiceLineId_TenantId",
                        columns: x => new { x.CustomerInvoiceLineId, x.TenantId },
                        principalTable: "CustomerInvoiceLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceSourceAllocations_SalesDeliveries_SalesDeliveryId_TenantId",
                        columns: x => new { x.SalesDeliveryId, x.TenantId },
                        principalTable: "SalesDeliveries",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceSourceAllocations_SalesDeliveryLines_SalesDeliveryLineId_TenantId",
                        columns: x => new { x.SalesDeliveryLineId, x.TenantId },
                        principalTable: "SalesDeliveryLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceSourceAllocations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceHistories_CustomerInvoiceId_CreatedAtUtc",
                table: "CustomerInvoiceHistories",
                columns: new[] { "CustomerInvoiceId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceHistories_CustomerInvoiceId_TenantId",
                table: "CustomerInvoiceHistories",
                columns: new[] { "CustomerInvoiceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceHistories_TenantId",
                table: "CustomerInvoiceHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_CustomerInvoiceId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "CustomerInvoiceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_MeasurementUnitId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_ProductId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_SalesOrderLineId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "SalesOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_SalesPriceListId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "SalesPriceListId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_SalesPriceRuleId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "SalesPriceRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_TaxRateId_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_TenantId",
                table: "CustomerInvoiceLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_TenantId_CustomerInvoiceId_SalesOrderLineId",
                table: "CustomerInvoiceLines",
                columns: new[] { "TenantId", "CustomerInvoiceId", "SalesOrderLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_CustomerAddressId_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "CustomerAddressId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_CustomerId_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_SalesOrderId_Status",
                table: "CustomerInvoices",
                columns: new[] { "SalesOrderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_SalesOrderId_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_TenantId",
                table: "CustomerInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_TenantId_InvoiceNumber",
                table: "CustomerInvoices",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_TenantId_RequestKey",
                table: "CustomerInvoices",
                columns: new[] { "TenantId", "RequestKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceSourceAllocations_CustomerInvoiceLineId_TenantId",
                table: "CustomerInvoiceSourceAllocations",
                columns: new[] { "CustomerInvoiceLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceSourceAllocations_SalesDeliveryId_TenantId",
                table: "CustomerInvoiceSourceAllocations",
                columns: new[] { "SalesDeliveryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceSourceAllocations_SalesDeliveryLineId_TenantId",
                table: "CustomerInvoiceSourceAllocations",
                columns: new[] { "SalesDeliveryLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceSourceAllocations_TenantId",
                table: "CustomerInvoiceSourceAllocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceSourceAllocations_TenantId_CustomerInvoiceLineId_SalesDeliveryLineId",
                table: "CustomerInvoiceSourceAllocations",
                columns: new[] { "TenantId", "CustomerInvoiceLineId", "SalesDeliveryLineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerInvoiceHistories");

            migrationBuilder.DropTable(
                name: "CustomerInvoiceSourceAllocations");

            migrationBuilder.DropTable(
                name: "CustomerInvoiceLines");

            migrationBuilder.DropTable(
                name: "CustomerInvoices");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SalesDeliveryLines_Id_TenantId",
                table: "SalesDeliveryLines");

            migrationBuilder.DropColumn(
                name: "SalesInvoicePolicy",
                table: "AccountingSettings");
        }
    }
}
