using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryBalancesAndDefaultLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryBalances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    OnHand = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Reserved = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryBalances", x => x.Id);
                    table.CheckConstraint("CK_InventoryBalances_Reserved", "[Reserved] >= 0 AND (([OnHand] >= 0 AND [Reserved] <= [OnHand]) OR ([OnHand] < 0 AND [Reserved] = 0))");
                    table.ForeignKey(
                        name: "FK_InventoryBalances_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBalances_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBalances_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBalances_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_ProductId_TenantId",
                table: "InventoryBalances",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_StorageLocationId_TenantId",
                table: "InventoryBalances",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_TenantId",
                table: "InventoryBalances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_TenantId_ProductId_WarehouseId",
                table: "InventoryBalances",
                columns: new[] { "TenantId", "ProductId", "WarehouseId" },
                unique: true,
                filter: "[StorageLocationId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_TenantId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryBalances",
                columns: new[] { "TenantId", "ProductId", "WarehouseId", "StorageLocationId" },
                unique: true,
                filter: "[StorageLocationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_WarehouseId_StorageLocationId",
                table: "InventoryBalances",
                columns: new[] { "WarehouseId", "StorageLocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBalances_WarehouseId_TenantId",
                table: "InventoryBalances",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM [ProductStocks] ps
                    CROSS APPLY (
                        SELECT COALESCE(SUM(pls.[Quantity]), 0) AS Assigned
                        FROM [ProductLocationStocks] pls
                        WHERE pls.[TenantId] = ps.[TenantId]
                          AND pls.[ProductId] = ps.[ProductId]
                          AND pls.[WarehouseId] = ps.[WarehouseId]
                    ) a
                    WHERE a.Assigned <> 0 AND a.Assigned - ps.[Quantity] > 0.000001
                )
                    THROW 51000, 'InventoryBalance backfill stopped because assigned location stock exceeds warehouse stock.', 1;

                INSERT INTO [InventoryBalances]
                    ([ProductId], [WarehouseId], [StorageLocationId], [OnHand], [Reserved], [UpdatedAt], [TenantId])
                SELECT pls.[ProductId], pls.[WarehouseId], pls.[StorageLocationId], pls.[Quantity], 0, SYSUTCDATETIME(), pls.[TenantId]
                FROM [ProductLocationStocks] pls
                WHERE EXISTS (
                    SELECT 1 FROM [ProductStocks] ps
                    WHERE ps.[TenantId] = pls.[TenantId]
                      AND ps.[ProductId] = pls.[ProductId]
                      AND ps.[WarehouseId] = pls.[WarehouseId]
                );

                INSERT INTO [InventoryBalances]
                    ([ProductId], [WarehouseId], [StorageLocationId], [OnHand], [Reserved], [UpdatedAt], [TenantId])
                SELECT ps.[ProductId], ps.[WarehouseId], NULL,
                       ps.[Quantity] - COALESCE((
                           SELECT SUM(pls.[Quantity])
                           FROM [ProductLocationStocks] pls
                           WHERE pls.[TenantId] = ps.[TenantId]
                             AND pls.[ProductId] = ps.[ProductId]
                             AND pls.[WarehouseId] = ps.[WarehouseId]
                       ), 0),
                       0, SYSUTCDATETIME(), ps.[TenantId]
                FROM [ProductStocks] ps;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryBalances");
        }
    }
}
