using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_SalesQuotationLines_Id_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceQuotationId = table.Column<int>(type: "int", nullable: false),
                    SourceQuotationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    ExchangeRateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExchangeRateSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentTermDays = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ConfirmedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                    table.UniqueConstraint("AK_SalesOrders_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SalesOrders_CustomerAddresses_CustomerAddressId_TenantId",
                        columns: x => new { x.CustomerAddressId, x.TenantId },
                        principalTable: "CustomerAddresses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrders_SalesQuotations_SourceQuotationId_TenantId",
                        columns: x => new { x.SourceQuotationId, x.TenantId },
                        principalTable: "SalesQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SalesOrderHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderHistories_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
                    SourceQuotationLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitFactorToStock = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SalesPriceListId = table.Column<int>(type: "int", nullable: true),
                    SalesPriceRuleId = table.Column<int>(type: "int", nullable: true),
                    PriceSourceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    CompanyUnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxRateId = table.Column<int>(type: "int", nullable: true),
                    TaxName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TaxRatePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    IsTaxInclusive = table.Column<bool>(type: "bit", nullable: false),
                    TaxSourceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TaxExemptionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_SalesPriceLists_SalesPriceListId_TenantId",
                        columns: x => new { x.SalesPriceListId, x.TenantId },
                        principalTable: "SalesPriceLists",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_SalesPriceRules_SalesPriceRuleId_TenantId",
                        columns: x => new { x.SalesPriceRuleId, x.TenantId },
                        principalTable: "SalesPriceRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_SalesQuotationLines_SourceQuotationLineId_TenantId",
                        columns: x => new { x.SourceQuotationLineId, x.TenantId },
                        principalTable: "SalesQuotationLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_TaxRates_TaxRateId_TenantId",
                        columns: x => new { x.TaxRateId, x.TenantId },
                        principalTable: "TaxRates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderHistories_SalesOrderId_CreatedAtUtc",
                table: "SalesOrderHistories",
                columns: new[] { "SalesOrderId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderHistories_SalesOrderId_TenantId",
                table: "SalesOrderHistories",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderHistories_TenantId",
                table: "SalesOrderHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_MeasurementUnitId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_ProductId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_SalesOrderId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_SalesPriceListId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "SalesPriceListId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_SalesPriceRuleId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "SalesPriceRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_SourceQuotationLineId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "SourceQuotationLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_TaxRateId_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_TenantId",
                table: "SalesOrderLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_TenantId_SourceQuotationLineId",
                table: "SalesOrderLines",
                columns: new[] { "TenantId", "SourceQuotationLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerAddressId_TenantId",
                table: "SalesOrders",
                columns: new[] { "CustomerAddressId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId_TenantId",
                table: "SalesOrders",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_SourceQuotationId_TenantId",
                table: "SalesOrders",
                columns: new[] { "SourceQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_Status_CreatedAtUtc",
                table: "SalesOrders",
                columns: new[] { "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_TenantId",
                table: "SalesOrders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_TenantId_OrderNumber",
                table: "SalesOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_TenantId_SourceQuotationId",
                table: "SalesOrders",
                columns: new[] { "TenantId", "SourceQuotationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesOrderHistories");

            migrationBuilder.DropTable(
                name: "SalesOrderLines");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SalesQuotationLines_Id_TenantId",
                table: "SalesQuotationLines");
        }
    }
}
