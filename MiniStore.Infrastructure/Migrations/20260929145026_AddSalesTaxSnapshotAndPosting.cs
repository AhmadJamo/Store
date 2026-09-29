using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesTaxSnapshotAndPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTaxInclusive",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxOutputAccountId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxRateId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRatePercent",
                table: "Sales",
                type: "decimal(9,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TaxRateId_TenantId",
                table: "Sales",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_TaxRates_TaxRateId_TenantId",
                table: "Sales",
                columns: new[] { "TaxRateId", "TenantId" },
                principalTable: "TaxRates",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_TaxRates_TaxRateId_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TaxRateId_TenantId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsTaxInclusive",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TaxOutputAccountId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TaxRateId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TaxRatePercent",
                table: "Sales");
        }
    }
}
