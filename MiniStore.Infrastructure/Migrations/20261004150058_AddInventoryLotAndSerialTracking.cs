using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryLotAndSerialTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryTrackingBalances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    Policy = table.Column<int>(type: "int", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ManufactureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTrackingBalances", x => x.Id);
                    table.UniqueConstraint("AK_InventoryTrackingBalances_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_InventoryTrackingBalances_Quantity", "[Quantity] >= 0");
                    table.CheckConstraint("CK_InventoryTrackingBalances_SerialQuantity", "[Policy] <> 2 OR [Quantity] IN (0,1)");
                    table.ForeignKey(
                        name: "FK_InventoryTrackingBalances_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingBalances_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingBalances_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingBalances_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTrackingTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryTrackingBalanceId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SourceReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTrackingTransactions", x => x.Id);
                    table.CheckConstraint("CK_InventoryTrackingTransactions_Quantity", "[Quantity] <> 0");
                    table.ForeignKey(
                        name: "FK_InventoryTrackingTransactions_InventoryTrackingBalances_InventoryTrackingBalanceId_TenantId",
                        columns: x => new { x.InventoryTrackingBalanceId, x.TenantId },
                        principalTable: "InventoryTrackingBalances",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingTransactions_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingTransactions_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTrackingTransactions_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_ProductId_TenantId",
                table: "InventoryTrackingBalances",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_StorageLocationId_TenantId",
                table: "InventoryTrackingBalances",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_TenantId",
                table: "InventoryTrackingBalances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_TenantId_ProductId_Policy_Identifier",
                table: "InventoryTrackingBalances",
                columns: new[] { "TenantId", "ProductId", "Policy", "Identifier" },
                unique: true,
                filter: "[Policy] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_TenantId_ProductId_Policy_Identifier_WarehouseId",
                table: "InventoryTrackingBalances",
                columns: new[] { "TenantId", "ProductId", "Policy", "Identifier", "WarehouseId" },
                unique: true,
                filter: "[Policy] = 1 AND [StorageLocationId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_TenantId_ProductId_Policy_Identifier_WarehouseId_StorageLocationId",
                table: "InventoryTrackingBalances",
                columns: new[] { "TenantId", "ProductId", "Policy", "Identifier", "WarehouseId", "StorageLocationId" },
                unique: true,
                filter: "[Policy] = 1 AND [StorageLocationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_WarehouseId_StorageLocationId_ExpirationDate",
                table: "InventoryTrackingBalances",
                columns: new[] { "WarehouseId", "StorageLocationId", "ExpirationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingBalances_WarehouseId_TenantId",
                table: "InventoryTrackingBalances",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_InventoryTrackingBalanceId_CreatedAt",
                table: "InventoryTrackingTransactions",
                columns: new[] { "InventoryTrackingBalanceId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_InventoryTrackingBalanceId_TenantId",
                table: "InventoryTrackingTransactions",
                columns: new[] { "InventoryTrackingBalanceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_ProductId_CreatedAt",
                table: "InventoryTrackingTransactions",
                columns: new[] { "ProductId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_ProductId_TenantId",
                table: "InventoryTrackingTransactions",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_StorageLocationId_TenantId",
                table: "InventoryTrackingTransactions",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_TenantId",
                table: "InventoryTrackingTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTrackingTransactions_WarehouseId_TenantId",
                table: "InventoryTrackingTransactions",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryTrackingTransactions");

            migrationBuilder.DropTable(
                name: "InventoryTrackingBalances");
        }
    }
}
