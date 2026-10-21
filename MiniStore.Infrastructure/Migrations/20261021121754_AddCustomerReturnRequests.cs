using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerReturnRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerReturnRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesDeliveryId = table.Column<int>(type: "int", nullable: false),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecidedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DecidedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelledByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReturnRequests", x => x.Id);
                    table.UniqueConstraint("AK_CustomerReturnRequests_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequests_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequests_SalesDeliveries_SalesDeliveryId_TenantId",
                        columns: x => new { x.SalesDeliveryId, x.TenantId },
                        principalTable: "SalesDeliveries",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequests_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequests_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReturnRequestHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerReturnRequestId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CustomerReturnRequestHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestHistories_CustomerReturnRequests_CustomerReturnRequestId_TenantId",
                        columns: x => new { x.CustomerReturnRequestId, x.TenantId },
                        principalTable: "CustomerReturnRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReturnRequestLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerReturnRequestId = table.Column<int>(type: "int", nullable: false),
                    SalesDeliveryLineId = table.Column<int>(type: "int", nullable: false),
                    SalesOrderLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitCostSnapshot = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    CostOfGoodsSoldSnapshot = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrackingAllocationsSnapshot = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReturnRequestLines", x => x.Id);
                    table.CheckConstraint("CK_CustomerReturnRequestLines_Quantity", "[StockQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_CustomerReturnRequests_CustomerReturnRequestId_TenantId",
                        columns: x => new { x.CustomerReturnRequestId, x.TenantId },
                        principalTable: "CustomerReturnRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_SalesDeliveryLines_SalesDeliveryLineId_TenantId",
                        columns: x => new { x.SalesDeliveryLineId, x.TenantId },
                        principalTable: "SalesDeliveryLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_SalesOrderLines_SalesOrderLineId_TenantId",
                        columns: x => new { x.SalesOrderLineId, x.TenantId },
                        principalTable: "SalesOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnRequestLines_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestHistories_CustomerReturnRequestId_CreatedAtUtc",
                table: "CustomerReturnRequestHistories",
                columns: new[] { "CustomerReturnRequestId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestHistories_CustomerReturnRequestId_TenantId",
                table: "CustomerReturnRequestHistories",
                columns: new[] { "CustomerReturnRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestHistories_TenantId",
                table: "CustomerReturnRequestHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_CustomerReturnRequestId_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "CustomerReturnRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_ProductId_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_SalesDeliveryLineId_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "SalesDeliveryLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_SalesOrderLineId_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "SalesOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_StorageLocationId_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_TenantId",
                table: "CustomerReturnRequestLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_TenantId_CustomerReturnRequestId_SalesDeliveryLineId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "TenantId", "CustomerReturnRequestId", "SalesDeliveryLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequestLines_WarehouseId_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequests_CustomerId_TenantId",
                table: "CustomerReturnRequests",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequests_SalesDeliveryId_Status",
                table: "CustomerReturnRequests",
                columns: new[] { "SalesDeliveryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequests_SalesDeliveryId_TenantId",
                table: "CustomerReturnRequests",
                columns: new[] { "SalesDeliveryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequests_SalesOrderId_TenantId",
                table: "CustomerReturnRequests",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequests_TenantId",
                table: "CustomerReturnRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnRequests_TenantId_ReturnNumber",
                table: "CustomerReturnRequests",
                columns: new[] { "TenantId", "ReturnNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReturnRequestHistories");

            migrationBuilder.DropTable(
                name: "CustomerReturnRequestLines");

            migrationBuilder.DropTable(
                name: "CustomerReturnRequests");
        }
    }
}
