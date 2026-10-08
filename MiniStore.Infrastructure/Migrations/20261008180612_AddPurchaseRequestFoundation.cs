using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseRequestFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    NeededByDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseRequests_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PurchaseRequestHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestHistories_PurchaseRequests_PurchaseRequestId_TenantId",
                        columns: x => new { x.PurchaseRequestId, x.TenantId },
                        principalTable: "PurchaseRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    SuggestedSupplierId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestLines_PurchaseRequests_PurchaseRequestId_TenantId",
                        columns: x => new { x.PurchaseRequestId, x.TenantId },
                        principalTable: "PurchaseRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestLines_Suppliers_SuggestedSupplierId_TenantId",
                        columns: x => new { x.SuggestedSupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestHistories_PurchaseRequestId_CreatedAtUtc",
                table: "PurchaseRequestHistories",
                columns: new[] { "PurchaseRequestId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestHistories_PurchaseRequestId_TenantId",
                table: "PurchaseRequestHistories",
                columns: new[] { "PurchaseRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestHistories_TenantId",
                table: "PurchaseRequestHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestLines_MeasurementUnitId_TenantId",
                table: "PurchaseRequestLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestLines_ProductId_TenantId",
                table: "PurchaseRequestLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestLines_PurchaseRequestId_TenantId",
                table: "PurchaseRequestLines",
                columns: new[] { "PurchaseRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestLines_SuggestedSupplierId_TenantId",
                table: "PurchaseRequestLines",
                columns: new[] { "SuggestedSupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestLines_TenantId",
                table: "PurchaseRequestLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestLines_TenantId_PurchaseRequestId_ProductId_MeasurementUnitId",
                table: "PurchaseRequestLines",
                columns: new[] { "TenantId", "PurchaseRequestId", "ProductId", "MeasurementUnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_TenantId",
                table: "PurchaseRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_TenantId_RequestNumber",
                table: "PurchaseRequests",
                columns: new[] { "TenantId", "RequestNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_WarehouseId_TenantId",
                table: "PurchaseRequests",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestHistories");

            migrationBuilder.DropTable(
                name: "PurchaseRequestLines");

            migrationBuilder.DropTable(
                name: "PurchaseRequests");
        }
    }
}
