using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalStockMovementKernel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_LocationMovements_Id_TenantId",
                table: "LocationMovements",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationMovementId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    FromStorageLocationId = table.Column<int>(type: "int", nullable: true),
                    ToStorageLocationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_LocationMovements_LocationMovementId_TenantId",
                        columns: x => new { x.LocationMovementId, x.TenantId },
                        principalTable: "LocationMovements",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_StorageLocations_FromStorageLocationId_TenantId",
                        columns: x => new { x.FromStorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_StorageLocations_ToStorageLocationId_TenantId",
                        columns: x => new { x.ToStorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_FromStorageLocationId_TenantId",
                table: "StockMovements",
                columns: new[] { "FromStorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_LocationMovementId_TenantId",
                table: "StockMovements",
                columns: new[] { "LocationMovementId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId_PostedAt",
                table: "StockMovements",
                columns: new[] { "ProductId", "PostedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId_TenantId",
                table: "StockMovements",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId",
                table: "StockMovements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_IdempotencyKey",
                table: "StockMovements",
                columns: new[] { "TenantId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_LocationMovementId",
                table: "StockMovements",
                columns: new[] { "TenantId", "LocationMovementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ToStorageLocationId_TenantId",
                table: "StockMovements",
                columns: new[] { "ToStorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseId_PostedAt",
                table: "StockMovements",
                columns: new[] { "WarehouseId", "PostedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseId_TenantId",
                table: "StockMovements",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_LocationMovements_Id_TenantId",
                table: "LocationMovements");
        }
    }
}
