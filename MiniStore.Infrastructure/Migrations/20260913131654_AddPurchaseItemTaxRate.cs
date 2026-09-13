using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseItemTaxRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxRateId",
                table: "PurchaseItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_TaxRateId",
                table: "PurchaseItems",
                column: "TaxRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_TaxRates_TaxRateId",
                table: "PurchaseItems",
                column: "TaxRateId",
                principalTable: "TaxRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_TaxRates_TaxRateId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_TaxRateId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "TaxRateId",
                table: "PurchaseItems");
        }
    }
}
