using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnassignedReservationUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "ProductId", "WarehouseId" },
                unique: true,
                filter: "[StorageLocationId] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId",
                table: "InventoryReservationLines");
        }
    }
}
