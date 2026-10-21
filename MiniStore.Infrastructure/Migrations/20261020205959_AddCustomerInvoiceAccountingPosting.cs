using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerInvoiceAccountingPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCurrencyCode",
                table: "CustomerInvoices",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyNetAmount",
                table: "CustomerInvoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyTaxAmount",
                table: "CustomerInvoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyTotalAmount",
                table: "CustomerInvoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedAtUtc",
                table: "CustomerInvoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostedByUserId",
                table: "CustomerInvoices",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivableAccountIdSnapshot",
                table: "CustomerInvoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReversalDate",
                table: "CustomerInvoices",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReversalReason",
                table: "CustomerInvoices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReversedAtUtc",
                table: "CustomerInvoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReversedByUserId",
                table: "CustomerInvoices",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesRevenueAccountIdSnapshot",
                table: "CustomerInvoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyNetAmount",
                table: "CustomerInvoiceLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyTaxAmount",
                table: "CustomerInvoiceLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyTotalAmount",
                table: "CustomerInvoiceLines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxOutputAccountIdSnapshot",
                table: "CustomerInvoiceLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_ReceivableAccountIdSnapshot_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "ReceivableAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_SalesRevenueAccountIdSnapshot_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "SalesRevenueAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceLines_TaxOutputAccountIdSnapshot_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "TaxOutputAccountIdSnapshot", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceLines_Accounts_TaxOutputAccountIdSnapshot_TenantId",
                table: "CustomerInvoiceLines",
                columns: new[] { "TaxOutputAccountIdSnapshot", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_Accounts_ReceivableAccountIdSnapshot_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "ReceivableAccountIdSnapshot", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_Accounts_SalesRevenueAccountIdSnapshot_TenantId",
                table: "CustomerInvoices",
                columns: new[] { "SalesRevenueAccountIdSnapshot", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceLines_Accounts_TaxOutputAccountIdSnapshot_TenantId",
                table: "CustomerInvoiceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_Accounts_ReceivableAccountIdSnapshot_TenantId",
                table: "CustomerInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_Accounts_SalesRevenueAccountIdSnapshot_TenantId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoices_ReceivableAccountIdSnapshot_TenantId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoices_SalesRevenueAccountIdSnapshot_TenantId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoiceLines_TaxOutputAccountIdSnapshot_TenantId",
                table: "CustomerInvoiceLines");

            migrationBuilder.DropColumn(
                name: "CompanyCurrencyCode",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "CompanyNetAmount",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "CompanyTaxAmount",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "CompanyTotalAmount",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "PostedAtUtc",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "PostedByUserId",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "ReceivableAccountIdSnapshot",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "ReversalDate",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "ReversalReason",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "ReversedAtUtc",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "ReversedByUserId",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "SalesRevenueAccountIdSnapshot",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "CompanyNetAmount",
                table: "CustomerInvoiceLines");

            migrationBuilder.DropColumn(
                name: "CompanyTaxAmount",
                table: "CustomerInvoiceLines");

            migrationBuilder.DropColumn(
                name: "CompanyTotalAmount",
                table: "CustomerInvoiceLines");

            migrationBuilder.DropColumn(
                name: "TaxOutputAccountIdSnapshot",
                table: "CustomerInvoiceLines");
        }
    }
}
