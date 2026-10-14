using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDemandAllocationAndDirectDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DropshipAddress",
                table: "PurchaseOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropshipCustomerReference",
                table: "PurchaseOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropshipRecipientName",
                table: "PurchaseOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FulfillmentRoute",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "DirectDeliveredQuantity",
                table: "PurchaseOrderLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PurchaseDemandAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: false),
                    SourceLineId = table.Column<long>(type: "bigint", nullable: true),
                    SourceReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AllocatedStockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDemandAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDemandAllocations_PurchaseOrderLines_PurchaseOrderLineId_TenantId",
                        columns: x => new { x.PurchaseOrderLineId, x.TenantId },
                        principalTable: "PurchaseOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseDemandAllocations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseDirectDeliveryEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProofReference = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDirectDeliveryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDirectDeliveryEvents_PurchaseOrderLines_PurchaseOrderLineId_TenantId",
                        columns: x => new { x.PurchaseOrderLineId, x.TenantId },
                        principalTable: "PurchaseOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseDirectDeliveryEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandAllocations_PurchaseOrderLineId_TenantId",
                table: "PurchaseDemandAllocations",
                columns: new[] { "PurchaseOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandAllocations_TenantId",
                table: "PurchaseDemandAllocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandAllocations_TenantId_PurchaseOrderLineId_SourceType_SourceId_SourceLineId",
                table: "PurchaseDemandAllocations",
                columns: new[] { "TenantId", "PurchaseOrderLineId", "SourceType", "SourceId", "SourceLineId" },
                unique: true,
                filter: "[SourceLineId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDirectDeliveryEvents_PurchaseOrderLineId_RecordedAtUtc",
                table: "PurchaseDirectDeliveryEvents",
                columns: new[] { "PurchaseOrderLineId", "RecordedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDirectDeliveryEvents_PurchaseOrderLineId_TenantId",
                table: "PurchaseDirectDeliveryEvents",
                columns: new[] { "PurchaseOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDirectDeliveryEvents_TenantId",
                table: "PurchaseDirectDeliveryEvents",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseDemandAllocations");

            migrationBuilder.DropTable(
                name: "PurchaseDirectDeliveryEvents");

            migrationBuilder.DropColumn(
                name: "DropshipAddress",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DropshipCustomerReference",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DropshipRecipientName",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "FulfillmentRoute",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DirectDeliveredQuantity",
                table: "PurchaseOrderLines");
        }
    }
}
