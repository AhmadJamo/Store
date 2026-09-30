using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHierarchicalStorageLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "StorageLocations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCountable",
                table: "StorageLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPickable",
                table: "StorageLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReceivable",
                table: "StorageLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReservable",
                table: "StorageLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShippable",
                table: "StorageLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StorageLocations",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentLocationId",
                table: "StorageLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "StorageLocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE [StorageLocations]
                SET [Name] = [Code],
                    [Sequence] = [Id],
                    [IsCountable] = 1,
                    [IsReceivable] = CASE WHEN [Type] IN (2, 5) THEN 1 ELSE 0 END,
                    [IsPickable] = CASE WHEN [Type] IN (1, 3) THEN 1 ELSE 0 END,
                    [IsReservable] = CASE WHEN [Type] IN (1, 3) THEN 1 ELSE 0 END,
                    [IsShippable] = CASE WHEN [Type] = 4 THEN 1 ELSE 0 END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_ParentLocationId_TenantId",
                table: "StorageLocations",
                columns: new[] { "ParentLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_TenantId_WarehouseId_Barcode",
                table: "StorageLocations",
                columns: new[] { "TenantId", "WarehouseId", "Barcode" },
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_WarehouseId_ParentLocationId_Sequence",
                table: "StorageLocations",
                columns: new[] { "WarehouseId", "ParentLocationId", "Sequence" });

            migrationBuilder.AddForeignKey(
                name: "FK_StorageLocations_StorageLocations_ParentLocationId_TenantId",
                table: "StorageLocations",
                columns: new[] { "ParentLocationId", "TenantId" },
                principalTable: "StorageLocations",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorageLocations_StorageLocations_ParentLocationId_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_ParentLocationId_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_TenantId_WarehouseId_Barcode",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_WarehouseId_ParentLocationId_Sequence",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "IsCountable",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "IsPickable",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "IsReceivable",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "IsReservable",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "IsShippable",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "ParentLocationId",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "StorageLocations");
        }
    }
}
