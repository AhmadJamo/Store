using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCommercialDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultTaxRateId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxExempt",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTermDays",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCurrencyCode",
                table: "Customers",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxExemptionReason",
                table: "Customers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DefaultTaxRateId_TenantId",
                table: "Customers",
                columns: new[] { "DefaultTaxRateId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_TaxRates_DefaultTaxRateId_TenantId",
                table: "Customers",
                columns: new[] { "DefaultTaxRateId", "TenantId" },
                principalTable: "TaxRates",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_TaxRates_DefaultTaxRateId_TenantId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DefaultTaxRateId_TenantId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultTaxRateId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsTaxExempt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PaymentTermDays",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferredCurrencyCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TaxExemptionReason",
                table: "Customers");
        }
    }
}
