using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMixedPurchaseBillingSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_GoodsReceiptLineId",
                table: "VendorBillLines");

            migrationBuilder.AlterColumn<int>(
                name: "GoodsReceiptLineId",
                table: "VendorBillLines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "BillingBasisQuantity",
                table: "VendorBillLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BillingPolicySnapshot",
                table: "VendorBillLines",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseExpenseAccountIdSnapshot",
                table: "VendorBillLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseServiceAcceptanceLineId",
                table: "VendorBillLines",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GoodsReceiptLineId",
                table: "PurchaseMatchExceptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql("""
                UPDATE [VendorBillLines]
                SET [BillingPolicySnapshot] = 2,
                    [BillingBasisQuantity] = [ReceiptClearingQuantity]
                WHERE [BillingBasisQuantity] = 0;
                """);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PurchaseServiceAcceptanceLines_Id_TenantId",
                table: "PurchaseServiceAcceptanceLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "VendorBillLines",
                columns: new[] { "PurchaseExpenseAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_PurchaseServiceAcceptanceLineId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "PurchaseServiceAcceptanceLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_GoodsReceiptLineId",
                table: "VendorBillLines",
                columns: new[] { "TenantId", "VendorBillId", "GoodsReceiptLineId" },
                unique: true,
                filter: "[GoodsReceiptLineId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_PurchaseOrderLineId",
                table: "VendorBillLines",
                columns: new[] { "TenantId", "VendorBillId", "PurchaseOrderLineId" },
                unique: true,
                filter: "[GoodsReceiptLineId] IS NULL AND [PurchaseServiceAcceptanceLineId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_PurchaseServiceAcceptanceLineId",
                table: "VendorBillLines",
                columns: new[] { "TenantId", "VendorBillId", "PurchaseServiceAcceptanceLineId" },
                unique: true,
                filter: "[PurchaseServiceAcceptanceLineId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBillLines_Accounts_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "VendorBillLines",
                columns: new[] { "PurchaseExpenseAccountIdSnapshot", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBillLines_PurchaseServiceAcceptanceLines_PurchaseServiceAcceptanceLineId_TenantId",
                table: "VendorBillLines",
                columns: new[] { "PurchaseServiceAcceptanceLineId", "TenantId" },
                principalTable: "PurchaseServiceAcceptanceLines",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorBillLines_Accounts_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "VendorBillLines");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorBillLines_PurchaseServiceAcceptanceLines_PurchaseServiceAcceptanceLineId_TenantId",
                table: "VendorBillLines");

            migrationBuilder.DropIndex(
                name: "IX_VendorBillLines_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "VendorBillLines");

            migrationBuilder.DropIndex(
                name: "IX_VendorBillLines_PurchaseServiceAcceptanceLineId_TenantId",
                table: "VendorBillLines");

            migrationBuilder.DropIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_GoodsReceiptLineId",
                table: "VendorBillLines");

            migrationBuilder.DropIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_PurchaseOrderLineId",
                table: "VendorBillLines");

            migrationBuilder.DropIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_PurchaseServiceAcceptanceLineId",
                table: "VendorBillLines");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PurchaseServiceAcceptanceLines_Id_TenantId",
                table: "PurchaseServiceAcceptanceLines");

            migrationBuilder.DropColumn(
                name: "BillingBasisQuantity",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "BillingPolicySnapshot",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "PurchaseExpenseAccountIdSnapshot",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "PurchaseServiceAcceptanceLineId",
                table: "VendorBillLines");

            migrationBuilder.AlterColumn<int>(
                name: "GoodsReceiptLineId",
                table: "VendorBillLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GoodsReceiptLineId",
                table: "PurchaseMatchExceptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillLines_TenantId_VendorBillId_GoodsReceiptLineId",
                table: "VendorBillLines",
                columns: new[] { "TenantId", "VendorBillId", "GoodsReceiptLineId" },
                unique: true);
        }
    }
}
