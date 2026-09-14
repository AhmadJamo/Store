using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryOperatingPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowPosSales",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "ControlMode",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<bool>(
                name: "EnforceLocationCapacity",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "PickingStrategy",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.AddColumn<bool>(
                name: "RequireDestinationLocationForTransfers",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireSourceLocationForTransfers",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "InventorySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DefaultWarehouseType = table.Column<int>(type: "int", nullable: false),
                    DefaultControlMode = table.Column<int>(type: "int", nullable: false),
                    DefaultPickingStrategy = table.Column<int>(type: "int", nullable: false),
                    DefaultAllowPosSales = table.Column<bool>(type: "bit", nullable: false),
                    DefaultEnforceLocationCapacity = table.Column<bool>(type: "bit", nullable: false),
                    DefaultRequireSourceLocationForTransfers = table.Column<bool>(type: "bit", nullable: false),
                    DefaultRequireDestinationLocationForTransfers = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySettings", x => x.Id);
                    table.CheckConstraint("CK_InventorySettings_SingletonKey", "[SingletonKey] = 1");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventorySettings_SingletonKey",
                table: "InventorySettings",
                column: "SingletonKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventorySettings");

            migrationBuilder.DropColumn(
                name: "AllowPosSales",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ControlMode",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "EnforceLocationCapacity",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "PickingStrategy",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "RequireDestinationLocationForTransfers",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "RequireSourceLocationForTransfers",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Warehouses");
        }
    }
}
