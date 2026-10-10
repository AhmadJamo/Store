using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceiptGrniAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoodsReceivedNotInvoicedAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_GoodsReceivedNotInvoicedAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "GoodsReceivedNotInvoicedAccountId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_GoodsReceivedNotInvoicedAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "GoodsReceivedNotInvoicedAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_GoodsReceivedNotInvoicedAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_GoodsReceivedNotInvoicedAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "GoodsReceivedNotInvoicedAccountId",
                table: "AccountingSettings");
        }
    }
}
