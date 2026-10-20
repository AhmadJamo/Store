using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrderReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_TenantId_SourceType_SourceId",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId",
                table: "InventoryReservationLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines");

            migrationBuilder.AddColumn<int>(
                name: "SourceLineId",
                table: "InventoryReservationLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_Tenant_Source_Status",
                table: "InventoryReservations",
                columns: new[] { "TenantId", "SourceType", "SourceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_InventoryReservations_StockTransferSource",
                table: "InventoryReservations",
                columns: new[] { "TenantId", "SourceType", "SourceId" },
                unique: true,
                filter: "[SourceType] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "ProductId", "WarehouseId" },
                unique: true,
                filter: "[SourceLineId] IS NULL AND [StorageLocationId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "ProductId", "WarehouseId", "StorageLocationId" },
                unique: true,
                filter: "[SourceLineId] IS NULL AND [StorageLocationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_SourceLineId_ProductId_WarehouseId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "SourceLineId", "ProductId", "WarehouseId" },
                unique: true,
                filter: "[SourceLineId] IS NOT NULL AND [StorageLocationId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_SourceLineId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "SourceLineId", "ProductId", "WarehouseId", "StorageLocationId" },
                unique: true,
                filter: "[SourceLineId] IS NOT NULL AND [StorageLocationId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_Tenant_Source_Status",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "UX_InventoryReservations_StockTransferSource",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId",
                table: "InventoryReservationLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_SourceLineId_ProductId_WarehouseId",
                table: "InventoryReservationLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_SourceLineId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines");

            migrationBuilder.DropColumn(
                name: "SourceLineId",
                table: "InventoryReservationLines");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_TenantId_SourceType_SourceId",
                table: "InventoryReservations",
                columns: new[] { "TenantId", "SourceType", "SourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "ProductId", "WarehouseId" },
                unique: true,
                filter: "[StorageLocationId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "ProductId", "WarehouseId", "StorageLocationId" },
                unique: true,
                filter: "[StorageLocationId] IS NOT NULL");
        }
    }
}
