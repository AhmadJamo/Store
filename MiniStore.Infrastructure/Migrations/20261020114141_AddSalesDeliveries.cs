using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsumedQuantity",
                table: "InventoryReservationLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SalesOrderLines_Id_TenantId",
                table: "SalesOrderLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_InventoryReservationLines_Id_TenantId",
                table: "InventoryReservationLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "SalesDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
                    InventoryReservationId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDeliveries", x => x.Id);
                    table.UniqueConstraint("AK_SalesDeliveries_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SalesDeliveries_InventoryReservations_InventoryReservationId_TenantId",
                        columns: x => new { x.InventoryReservationId, x.TenantId },
                        principalTable: "InventoryReservations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveries_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveries_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveries_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesDeliveryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesDeliveryId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDeliveryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryHistories_SalesDeliveries_SalesDeliveryId_TenantId",
                        columns: x => new { x.SalesDeliveryId, x.TenantId },
                        principalTable: "SalesDeliveries",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesDeliveryLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesDeliveryId = table.Column<int>(type: "int", nullable: false),
                    SalesOrderLineId = table.Column<int>(type: "int", nullable: false),
                    InventoryReservationLineId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    CostOfGoodsSold = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrackingAllocations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsCostPosted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDeliveryLines", x => x.Id);
                    table.CheckConstraint("CK_SalesDeliveryLines_Quantity", "[StockQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_InventoryReservationLines_InventoryReservationLineId_TenantId",
                        columns: x => new { x.InventoryReservationLineId, x.TenantId },
                        principalTable: "InventoryReservationLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_SalesDeliveries_SalesDeliveryId_TenantId",
                        columns: x => new { x.SalesDeliveryId, x.TenantId },
                        principalTable: "SalesDeliveries",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_SalesOrderLines_SalesOrderLineId_TenantId",
                        columns: x => new { x.SalesOrderLineId, x.TenantId },
                        principalTable: "SalesOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryLines_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_InventoryReservationId_TenantId",
                table: "SalesDeliveries",
                columns: new[] { "InventoryReservationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_SalesOrderId_Status",
                table: "SalesDeliveries",
                columns: new[] { "SalesOrderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_SalesOrderId_TenantId",
                table: "SalesDeliveries",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_TenantId",
                table: "SalesDeliveries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_TenantId_DeliveryNumber",
                table: "SalesDeliveries",
                columns: new[] { "TenantId", "DeliveryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_TenantId_InventoryReservationId",
                table: "SalesDeliveries",
                columns: new[] { "TenantId", "InventoryReservationId" },
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveries_WarehouseId_TenantId",
                table: "SalesDeliveries",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryHistories_SalesDeliveryId_CreatedAtUtc",
                table: "SalesDeliveryHistories",
                columns: new[] { "SalesDeliveryId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryHistories_SalesDeliveryId_TenantId",
                table: "SalesDeliveryHistories",
                columns: new[] { "SalesDeliveryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryHistories_TenantId",
                table: "SalesDeliveryHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_InventoryReservationLineId_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "InventoryReservationLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_ProductId_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_SalesDeliveryId_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "SalesDeliveryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_SalesOrderLineId_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "SalesOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_StorageLocationId_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_TenantId",
                table: "SalesDeliveryLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_TenantId_SalesDeliveryId_InventoryReservationLineId",
                table: "SalesDeliveryLines",
                columns: new[] { "TenantId", "SalesDeliveryId", "InventoryReservationLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryLines_WarehouseId_TenantId",
                table: "SalesDeliveryLines",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesDeliveryHistories");

            migrationBuilder.DropTable(
                name: "SalesDeliveryLines");

            migrationBuilder.DropTable(
                name: "SalesDeliveries");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SalesOrderLines_Id_TenantId",
                table: "SalesOrderLines");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_InventoryReservationLines_Id_TenantId",
                table: "InventoryReservationLines");

            migrationBuilder.DropColumn(
                name: "ConsumedQuantity",
                table: "InventoryReservationLines");
        }
    }
}
