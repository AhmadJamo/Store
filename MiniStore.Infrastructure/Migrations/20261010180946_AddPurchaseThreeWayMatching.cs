using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseThreeWayMatching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptClearingQuantity",
                table: "VendorBillLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.Sql("UPDATE [VendorBillLines] SET [ReceiptClearingQuantity] = [Quantity] WHERE [ReceiptClearingQuantity] IS NULL");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReceiptClearingQuantity",
                table: "VendorBillLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_VendorBillLines_Id_TenantId",
                table: "VendorBillLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "PurchaseMatchingSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    QuantityTolerancePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    PriceTolerancePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseMatchingSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchingSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseMatchRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorBillId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    QuantityTolerancePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    PriceTolerancePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    RunAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RunByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    WasOverridden = table.Column<bool>(type: "bit", nullable: false),
                    OverrideReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OverriddenByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    OverriddenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseMatchRuns", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseMatchRuns_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseMatchRuns_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchRuns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchRuns_VendorBills_VendorBillId_TenantId",
                        columns: x => new { x.VendorBillId, x.TenantId },
                        principalTable: "VendorBills",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseMatchExceptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseMatchRunId = table.Column<int>(type: "int", nullable: false),
                    VendorBillLineId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false),
                    GoodsReceiptLineId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ExpectedValue = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    VariancePercent = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TolerancePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseMatchExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchExceptions_GoodsReceiptLines_GoodsReceiptLineId_TenantId",
                        columns: x => new { x.GoodsReceiptLineId, x.TenantId },
                        principalTable: "GoodsReceiptLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchExceptions_PurchaseMatchRuns_PurchaseMatchRunId_TenantId",
                        columns: x => new { x.PurchaseMatchRunId, x.TenantId },
                        principalTable: "PurchaseMatchRuns",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchExceptions_PurchaseOrderLines_PurchaseOrderLineId_TenantId",
                        columns: x => new { x.PurchaseOrderLineId, x.TenantId },
                        principalTable: "PurchaseOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchExceptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseMatchExceptions_VendorBillLines_VendorBillLineId_TenantId",
                        columns: x => new { x.VendorBillLineId, x.TenantId },
                        principalTable: "VendorBillLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchExceptions_GoodsReceiptLineId_TenantId",
                table: "PurchaseMatchExceptions",
                columns: new[] { "GoodsReceiptLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchExceptions_PurchaseMatchRunId_TenantId",
                table: "PurchaseMatchExceptions",
                columns: new[] { "PurchaseMatchRunId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchExceptions_PurchaseOrderLineId_TenantId",
                table: "PurchaseMatchExceptions",
                columns: new[] { "PurchaseOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchExceptions_TenantId",
                table: "PurchaseMatchExceptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchExceptions_TenantId_PurchaseMatchRunId_VendorBillLineId_Type",
                table: "PurchaseMatchExceptions",
                columns: new[] { "TenantId", "PurchaseMatchRunId", "VendorBillLineId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchExceptions_VendorBillLineId_TenantId",
                table: "PurchaseMatchExceptions",
                columns: new[] { "VendorBillLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchingSettings_TenantId",
                table: "PurchaseMatchingSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchingSettings_TenantId_SingletonKey",
                table: "PurchaseMatchingSettings",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchRuns_PurchaseOrderId_TenantId",
                table: "PurchaseMatchRuns",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchRuns_TenantId",
                table: "PurchaseMatchRuns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchRuns_TenantId_VendorBillId",
                table: "PurchaseMatchRuns",
                columns: new[] { "TenantId", "VendorBillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMatchRuns_VendorBillId_TenantId",
                table: "PurchaseMatchRuns",
                columns: new[] { "VendorBillId", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseMatchExceptions");

            migrationBuilder.DropTable(
                name: "PurchaseMatchingSettings");

            migrationBuilder.DropTable(
                name: "PurchaseMatchRuns");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_VendorBillLines_Id_TenantId",
                table: "VendorBillLines");

            migrationBuilder.DropColumn(
                name: "ReceiptClearingQuantity",
                table: "VendorBillLines");
        }
    }
}
