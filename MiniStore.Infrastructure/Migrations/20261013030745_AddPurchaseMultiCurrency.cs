using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseMultiCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCurrencyCode",
                table: "VendorBills",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "VendorBills",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateDate",
                table: "VendorBills",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateSource",
                table: "VendorBills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyGrossAmount",
                table: "VendorBillLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyNetAmount",
                table: "VendorBillLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyTaxAmount",
                table: "VendorBillLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptExchangeRate",
                table: "VendorBillLines",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCurrencyCode",
                table: "SupplierPayments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "SupplierPayments",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateDate",
                table: "SupplierPayments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateSource",
                table: "SupplierPayments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyCurrencyAmount",
                table: "SupplierPaymentLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PayableCarryingAmount",
                table: "SupplierPaymentLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyGrossAmount",
                table: "SupplierCreditNoteLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyNetAmount",
                table: "SupplierCreditNoteLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyTaxAmount",
                table: "SupplierCreditNoteLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCurrencyCode",
                table: "PurchaseOrders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "PurchaseOrders",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateDate",
                table: "PurchaseOrders",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateSource",
                table: "PurchaseOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCurrencyCode",
                table: "GoodsReceipts",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GoodsReceipts",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "GoodsReceipts",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateDate",
                table: "GoodsReceipts",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateSource",
                table: "GoodsReceipts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RealizedExchangeGainAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealizedExchangeLossAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE po SET CompanyCurrencyCode = COALESCE(gs.Currency, po.CurrencyCode), ExchangeRate = 1,
    ExchangeRateDate = po.OrderDate, ExchangeRateSource = 'Legacy company-currency value'
FROM PurchaseOrders po
LEFT JOIN GeneralSettings gs ON gs.TenantId = po.TenantId AND gs.SingletonKey = 1;

UPDATE gr SET CurrencyCode = po.CurrencyCode, CompanyCurrencyCode = COALESCE(gs.Currency, po.CurrencyCode),
    ExchangeRate = 1, ExchangeRateDate = gr.ReceiptDate, ExchangeRateSource = 'Legacy company-currency value'
FROM GoodsReceipts gr
JOIN PurchaseOrders po ON po.Id = gr.PurchaseOrderId AND po.TenantId = gr.TenantId
LEFT JOIN GeneralSettings gs ON gs.TenantId = gr.TenantId AND gs.SingletonKey = 1;

UPDATE vb SET CompanyCurrencyCode = COALESCE(gs.Currency, vb.CurrencyCode), ExchangeRate = 1,
    ExchangeRateDate = vb.BillDate, ExchangeRateSource = 'Legacy company-currency value'
FROM VendorBills vb
LEFT JOIN GeneralSettings gs ON gs.TenantId = vb.TenantId AND gs.SingletonKey = 1;
UPDATE VendorBillLines SET CompanyNetAmount = NetAmount, CompanyTaxAmount = TaxAmount,
    CompanyGrossAmount = GrossAmount, ReceiptExchangeRate = 1;

UPDATE sp SET CompanyCurrencyCode = COALESCE(gs.Currency, sp.CurrencyCode), ExchangeRate = 1,
    ExchangeRateDate = sp.PaymentDate, ExchangeRateSource = 'Legacy company-currency value'
FROM SupplierPayments sp
LEFT JOIN GeneralSettings gs ON gs.TenantId = sp.TenantId AND gs.SingletonKey = 1;
UPDATE SupplierPaymentLines SET CompanyCurrencyAmount = Amount, PayableCarryingAmount = Amount;
UPDATE SupplierCreditNoteLines SET CompanyNetAmount = NetAmount, CompanyTaxAmount = TaxAmount, CompanyGrossAmount = GrossAmount;");

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CompanyCurrencyPerUnit = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_RealizedExchangeGainAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "RealizedExchangeGainAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_RealizedExchangeLossAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "RealizedExchangeLossAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_TenantId",
                table: "CurrencyExchangeRates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_TenantId_CurrencyCode_EffectiveDate",
                table: "CurrencyExchangeRates",
                columns: new[] { "TenantId", "CurrencyCode", "EffectiveDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_RealizedExchangeGainAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "RealizedExchangeGainAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_RealizedExchangeLossAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "RealizedExchangeLossAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_RealizedExchangeGainAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_RealizedExchangeLossAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_RealizedExchangeGainAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_RealizedExchangeLossAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "CompanyCurrencyCode",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "ExchangeRateSource",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "CompanyGrossAmount",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "CompanyNetAmount",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "CompanyTaxAmount",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "ReceiptExchangeRate",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "CompanyCurrencyCode",
                table: "SupplierPayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "SupplierPayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "SupplierPayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateSource",
                table: "SupplierPayments");

            migrationBuilder.DropColumn(
                name: "CompanyCurrencyAmount",
                table: "SupplierPaymentLines");

            migrationBuilder.DropColumn(
                name: "PayableCarryingAmount",
                table: "SupplierPaymentLines");

            migrationBuilder.DropColumn(
                name: "CompanyGrossAmount",
                table: "SupplierCreditNoteLines");

            migrationBuilder.DropColumn(
                name: "CompanyNetAmount",
                table: "SupplierCreditNoteLines");

            migrationBuilder.DropColumn(
                name: "CompanyTaxAmount",
                table: "SupplierCreditNoteLines");

            migrationBuilder.DropColumn(
                name: "CompanyCurrencyCode",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ExchangeRateSource",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "CompanyCurrencyCode",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "ExchangeRateSource",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "RealizedExchangeGainAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "RealizedExchangeLossAccountId",
                table: "AccountingSettings");
        }
    }
}
