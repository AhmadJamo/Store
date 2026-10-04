using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryReservations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    SourceReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleaseReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryReservations", x => x.Id);
                    table.UniqueConstraint("AK_InventoryReservations_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_InventoryReservations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryReservationLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryReservationId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryReservationLines", x => x.Id);
                    table.CheckConstraint("CK_InventoryReservationLines_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_InventoryReservationLines_InventoryReservations_InventoryReservationId_TenantId",
                        columns: x => new { x.InventoryReservationId, x.TenantId },
                        principalTable: "InventoryReservations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryReservationLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryReservationLines_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryReservationLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryReservationLines_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_InventoryReservationId_TenantId",
                table: "InventoryReservationLines",
                columns: new[] { "InventoryReservationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_ProductId_TenantId",
                table: "InventoryReservationLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_StorageLocationId_TenantId",
                table: "InventoryReservationLines",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId",
                table: "InventoryReservationLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_TenantId_InventoryReservationId_ProductId_WarehouseId_StorageLocationId",
                table: "InventoryReservationLines",
                columns: new[] { "TenantId", "InventoryReservationId", "ProductId", "WarehouseId", "StorageLocationId" },
                unique: true,
                filter: "[StorageLocationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_WarehouseId_StorageLocationId_ProductId",
                table: "InventoryReservationLines",
                columns: new[] { "WarehouseId", "StorageLocationId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationLines_WarehouseId_TenantId",
                table: "InventoryReservationLines",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_Status_CreatedAt",
                table: "InventoryReservations",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_TenantId",
                table: "InventoryReservations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_TenantId_SourceType_SourceId",
                table: "InventoryReservations",
                columns: new[] { "TenantId", "SourceType", "SourceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryReservationLines");

            migrationBuilder.DropTable(
                name: "InventoryReservations");
        }
    }
}
