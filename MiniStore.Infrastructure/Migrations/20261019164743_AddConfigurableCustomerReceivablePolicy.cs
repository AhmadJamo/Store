using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurableCustomerReceivablePolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_AccountId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "CustomerAccountCodePrefix",
                table: "AccountingSettings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "CUST-");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CustomerPolicyEffectiveFrom",
                table: "AccountingSettings",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerReceivableControlAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerReceivableMode",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerReceivableParentAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_CustomerReceivableControlAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "CustomerReceivableControlAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_CustomerReceivableParentAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "CustomerReceivableParentAccountId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_CustomerReceivableControlAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "CustomerReceivableControlAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_CustomerReceivableParentAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "CustomerReceivableParentAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_CustomerReceivableControlAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_CustomerReceivableParentAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_CustomerReceivableControlAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_CustomerReceivableParentAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "CustomerAccountCodePrefix",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "CustomerPolicyEffectiveFrom",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "CustomerReceivableControlAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "CustomerReceivableMode",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "CustomerReceivableParentAccountId",
                table: "AccountingSettings");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_AccountId",
                table: "Customers",
                columns: new[] { "TenantId", "AccountId" },
                unique: true);
        }
    }
}
