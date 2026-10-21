using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerReturnReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_CustomerReturnRequestLines_Id_TenantId",
                table: "CustomerReturnRequestLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "CustomerReturnReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerReturnRequestId = table.Column<int>(type: "int", nullable: false),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspectedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    InspectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReturnReceipts", x => x.Id);
                    table.UniqueConstraint("AK_CustomerReturnReceipts_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceipts_CustomerReturnRequests_CustomerReturnRequestId_TenantId",
                        columns: x => new { x.CustomerReturnRequestId, x.TenantId },
                        principalTable: "CustomerReturnRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceipts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReturnReceiptHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerReturnReceiptId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CustomerReturnReceiptHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptHistories_CustomerReturnReceipts_CustomerReturnReceiptId_TenantId",
                        columns: x => new { x.CustomerReturnReceiptId, x.TenantId },
                        principalTable: "CustomerReturnReceipts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReturnReceiptLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerReturnReceiptId = table.Column<int>(type: "int", nullable: false),
                    CustomerReturnRequestLineId = table.Column<int>(type: "int", nullable: false),
                    SalesDeliveryLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: true),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Disposition = table.Column<int>(type: "int", nullable: false),
                    InspectionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrackingAllocations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UnitCostSnapshot = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReturnReceiptLines", x => x.Id);
                    table.CheckConstraint("CK_CustomerReturnReceiptLines_Quantity", "[ReceivedQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_CustomerReturnReceipts_CustomerReturnReceiptId_TenantId",
                        columns: x => new { x.CustomerReturnReceiptId, x.TenantId },
                        principalTable: "CustomerReturnReceipts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_CustomerReturnRequestLines_CustomerReturnRequestLineId_TenantId",
                        columns: x => new { x.CustomerReturnRequestLineId, x.TenantId },
                        principalTable: "CustomerReturnRequestLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_SalesDeliveryLines_SalesDeliveryLineId_TenantId",
                        columns: x => new { x.SalesDeliveryLineId, x.TenantId },
                        principalTable: "SalesDeliveryLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReturnReceiptLines_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptHistories_CustomerReturnReceiptId_CreatedAtUtc",
                table: "CustomerReturnReceiptHistories",
                columns: new[] { "CustomerReturnReceiptId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptHistories_CustomerReturnReceiptId_TenantId",
                table: "CustomerReturnReceiptHistories",
                columns: new[] { "CustomerReturnReceiptId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptHistories_TenantId",
                table: "CustomerReturnReceiptHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_CustomerReturnReceiptId_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "CustomerReturnReceiptId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_CustomerReturnRequestLineId_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "CustomerReturnRequestLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_ProductId_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_SalesDeliveryLineId_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "SalesDeliveryLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_StorageLocationId_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_TenantId",
                table: "CustomerReturnReceiptLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_TenantId_CustomerReturnReceiptId_CustomerReturnRequestLineId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "TenantId", "CustomerReturnReceiptId", "CustomerReturnRequestLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceiptLines_WarehouseId_TenantId",
                table: "CustomerReturnReceiptLines",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceipts_CustomerReturnRequestId_Status",
                table: "CustomerReturnReceipts",
                columns: new[] { "CustomerReturnRequestId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceipts_CustomerReturnRequestId_TenantId",
                table: "CustomerReturnReceipts",
                columns: new[] { "CustomerReturnRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceipts_TenantId",
                table: "CustomerReturnReceipts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReturnReceipts_TenantId_ReceiptNumber",
                table: "CustomerReturnReceipts",
                columns: new[] { "TenantId", "ReceiptNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReturnReceiptHistories");

            migrationBuilder.DropTable(
                name: "CustomerReturnReceiptLines");

            migrationBuilder.DropTable(
                name: "CustomerReturnReceipts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CustomerReturnRequestLines_Id_TenantId",
                table: "CustomerReturnRequestLines");
        }
    }
}
