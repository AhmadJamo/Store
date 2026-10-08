using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseSourcingFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_PurchaseRequestLines_Id_TenantId",
                table: "PurchaseRequestLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "PurchaseSourcingEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourcingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSourcingEvents", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseSourcingEvents_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingEvents_PurchaseRequests_PurchaseRequestId_TenantId",
                        columns: x => new { x.PurchaseRequestId, x.TenantId },
                        principalTable: "PurchaseRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingEvents_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseSourcingLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: false),
                    PurchaseRequestLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSourcingLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingLines_PurchaseRequestLines_PurchaseRequestLineId_TenantId",
                        columns: x => new { x.PurchaseRequestLineId, x.TenantId },
                        principalTable: "PurchaseRequestLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingLines_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseSupplierInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSupplierInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseSupplierInvitations_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseSupplierInvitations_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseSupplierInvitations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_PurchaseRequestId_TenantId",
                table: "PurchaseSourcingEvents",
                columns: new[] { "PurchaseRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_TenantId",
                table: "PurchaseSourcingEvents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_TenantId_PurchaseRequestId",
                table: "PurchaseSourcingEvents",
                columns: new[] { "TenantId", "PurchaseRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_TenantId_SourcingNumber",
                table: "PurchaseSourcingEvents",
                columns: new[] { "TenantId", "SourcingNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_WarehouseId_TenantId",
                table: "PurchaseSourcingEvents",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_MeasurementUnitId_TenantId",
                table: "PurchaseSourcingLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_ProductId_TenantId",
                table: "PurchaseSourcingLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_PurchaseRequestLineId_TenantId",
                table: "PurchaseSourcingLines",
                columns: new[] { "PurchaseRequestLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_PurchaseSourcingEventId_TenantId",
                table: "PurchaseSourcingLines",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_TenantId",
                table: "PurchaseSourcingLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_TenantId_PurchaseSourcingEventId_PurchaseRequestLineId",
                table: "PurchaseSourcingLines",
                columns: new[] { "TenantId", "PurchaseSourcingEventId", "PurchaseRequestLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSupplierInvitations_PurchaseSourcingEventId_TenantId",
                table: "PurchaseSupplierInvitations",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSupplierInvitations_SupplierId_TenantId",
                table: "PurchaseSupplierInvitations",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSupplierInvitations_TenantId",
                table: "PurchaseSupplierInvitations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSupplierInvitations_TenantId_PurchaseSourcingEventId_SupplierId",
                table: "PurchaseSupplierInvitations",
                columns: new[] { "TenantId", "PurchaseSourcingEventId", "SupplierId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseSourcingLines");

            migrationBuilder.DropTable(
                name: "PurchaseSupplierInvitations");

            migrationBuilder.DropTable(
                name: "PurchaseSourcingEvents");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PurchaseRequestLines_Id_TenantId",
                table: "PurchaseRequestLines");
        }
    }
}
