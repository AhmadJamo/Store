using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesQuotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_SalesPriceRules_Id_TenantId",
                table: "SalesPriceRules",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CustomerAddresses_Id_TenantId",
                table: "CustomerAddresses",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "SalesQuotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestKey = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
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
                    QuotationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: false),
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
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesQuotations", x => x.Id);
                    table.UniqueConstraint("AK_SalesQuotations_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SalesQuotations_CustomerAddresses_CustomerAddressId_TenantId",
                        columns: x => new { x.CustomerAddressId, x.TenantId },
                        principalTable: "CustomerAddresses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotations_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesQuotationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesQuotationId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SalesQuotationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesQuotationHistories_SalesQuotations_SalesQuotationId_TenantId",
                        columns: x => new { x.SalesQuotationId, x.TenantId },
                        principalTable: "SalesQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesQuotationHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesQuotationLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesQuotationId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SalesQuotationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_SalesPriceLists_SalesPriceListId_TenantId",
                        columns: x => new { x.SalesPriceListId, x.TenantId },
                        principalTable: "SalesPriceLists",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_SalesPriceRules_SalesPriceRuleId_TenantId",
                        columns: x => new { x.SalesPriceRuleId, x.TenantId },
                        principalTable: "SalesPriceRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_SalesQuotations_SalesQuotationId_TenantId",
                        columns: x => new { x.SalesQuotationId, x.TenantId },
                        principalTable: "SalesQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_TaxRates_TaxRateId_TenantId",
                        columns: x => new { x.TaxRateId, x.TenantId },
                        principalTable: "TaxRates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationHistories_SalesQuotationId_CreatedAtUtc",
                table: "SalesQuotationHistories",
                columns: new[] { "SalesQuotationId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationHistories_SalesQuotationId_TenantId",
                table: "SalesQuotationHistories",
                columns: new[] { "SalesQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationHistories_TenantId",
                table: "SalesQuotationHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_MeasurementUnitId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_ProductId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_SalesPriceListId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "SalesPriceListId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_SalesPriceRuleId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "SalesPriceRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_SalesQuotationId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "SalesQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_TaxRateId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_TenantId",
                table: "SalesQuotationLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_TenantId_SalesQuotationId_ProductId_MeasurementUnitId",
                table: "SalesQuotationLines",
                columns: new[] { "TenantId", "SalesQuotationId", "ProductId", "MeasurementUnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_CustomerAddressId_TenantId",
                table: "SalesQuotations",
                columns: new[] { "CustomerAddressId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_CustomerId_TenantId",
                table: "SalesQuotations",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_Status_CreatedAtUtc",
                table: "SalesQuotations",
                columns: new[] { "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_TenantId",
                table: "SalesQuotations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_TenantId_QuotationNumber",
                table: "SalesQuotations",
                columns: new[] { "TenantId", "QuotationNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_TenantId_RequestKey",
                table: "SalesQuotations",
                columns: new[] { "TenantId", "RequestKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesQuotationHistories");

            migrationBuilder.DropTable(
                name: "SalesQuotationLines");

            migrationBuilder.DropTable(
                name: "SalesQuotations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SalesPriceRules_Id_TenantId",
                table: "SalesPriceRules");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CustomerAddresses_Id_TenantId",
                table: "CustomerAddresses");
        }
    }
}
