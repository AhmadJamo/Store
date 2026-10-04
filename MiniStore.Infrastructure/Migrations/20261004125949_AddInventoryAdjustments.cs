using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryAdjustments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjustmentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    IsBlindCount = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CountedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustments", x => x.Id);
                    table.UniqueConstraint("AK_InventoryAdjustments_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustmentLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryAdjustmentId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ExpectedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustmentLines", x => x.Id);
                    table.CheckConstraint("CK_InventoryAdjustmentLines_Counted", "[CountedQuantity] IS NULL OR [CountedQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentLines_InventoryAdjustments_InventoryAdjustmentId_TenantId",
                        columns: x => new { x.InventoryAdjustmentId, x.TenantId },
                        principalTable: "InventoryAdjustments",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentLines_InventoryAdjustmentId_TenantId",
                table: "InventoryAdjustmentLines",
                columns: new[] { "InventoryAdjustmentId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentLines_ProductId_TenantId",
                table: "InventoryAdjustmentLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentLines_TenantId",
                table: "InventoryAdjustmentLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentLines_TenantId_InventoryAdjustmentId_ProductId",
                table: "InventoryAdjustmentLines",
                columns: new[] { "TenantId", "InventoryAdjustmentId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_StorageLocationId_TenantId",
                table: "InventoryAdjustments",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_TenantId",
                table: "InventoryAdjustments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_TenantId_AdjustmentNumber",
                table: "InventoryAdjustments",
                columns: new[] { "TenantId", "AdjustmentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_WarehouseId_Status_CreatedAt",
                table: "InventoryAdjustments",
                columns: new[] { "WarehouseId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_WarehouseId_TenantId",
                table: "InventoryAdjustments",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryAdjustmentLines");

            migrationBuilder.DropTable(
                name: "InventoryAdjustments");
        }
    }
}
