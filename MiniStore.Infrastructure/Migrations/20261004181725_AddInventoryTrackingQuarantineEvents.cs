using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTrackingQuarantineEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryTrackingTransactions_Quantity",
                table: "InventoryTrackingTransactions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryTrackingTransactions_Quantity",
                table: "InventoryTrackingTransactions",
                sql: "[Quantity] <> 0 OR [Type] IN (7,8)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryTrackingTransactions_Quantity",
                table: "InventoryTrackingTransactions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryTrackingTransactions_Quantity",
                table: "InventoryTrackingTransactions",
                sql: "[Quantity] <> 0");
        }
    }
}
