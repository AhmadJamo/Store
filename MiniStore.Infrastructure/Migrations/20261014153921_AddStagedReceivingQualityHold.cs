using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStagedReceivingQualityHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseReceivingRoute",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "InspectedAtUtc",
                table: "GoodsReceipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectedByUserId",
                table: "GoodsReceipts",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionNotes",
                table: "GoodsReceipts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivingRouteSnapshot",
                table: "GoodsReceipts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "AcceptedQuantity",
                table: "GoodsReceiptLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DamagedQuantity",
                table: "GoodsReceiptLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveredQuantity",
                table: "GoodsReceiptLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscrepancyReason",
                table: "GoodsReceiptLines",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RejectedQuantity",
                table: "GoodsReceiptLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ShortQuantity",
                table: "GoodsReceiptLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE [GoodsReceiptLines]
                SET [DeliveredQuantity] = [ReceivedQuantity],
                    [AcceptedQuantity] = [ReceivedQuantity];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_TenantId_PurchaseOrderId",
                table: "GoodsReceipts",
                columns: new[] { "TenantId", "PurchaseOrderId" },
                unique: true,
                filter: "[Status] IN (5, 6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipts_TenantId_PurchaseOrderId",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "PurchaseReceivingRoute",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "InspectedAtUtc",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "InspectedByUserId",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "InspectionNotes",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "ReceivingRouteSnapshot",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "AcceptedQuantity",
                table: "GoodsReceiptLines");

            migrationBuilder.DropColumn(
                name: "DamagedQuantity",
                table: "GoodsReceiptLines");

            migrationBuilder.DropColumn(
                name: "DeliveredQuantity",
                table: "GoodsReceiptLines");

            migrationBuilder.DropColumn(
                name: "DiscrepancyReason",
                table: "GoodsReceiptLines");

            migrationBuilder.DropColumn(
                name: "RejectedQuantity",
                table: "GoodsReceiptLines");

            migrationBuilder.DropColumn(
                name: "ShortQuantity",
                table: "GoodsReceiptLines");
        }
    }
}
