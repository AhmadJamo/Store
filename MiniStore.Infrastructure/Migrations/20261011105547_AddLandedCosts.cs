using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLandedCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LandedCostClearingAccountId",
                table: "AccountingSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LandedCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GoodsReceiptId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    CostDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NormalizedExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AllocationMethod = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandedCosts", x => x.Id);
                    table.UniqueConstraint("AK_LandedCosts_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_LandedCosts_GoodsReceipts_GoodsReceiptId_TenantId",
                        columns: x => new { x.GoodsReceiptId, x.TenantId },
                        principalTable: "GoodsReceipts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LandedCosts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LandedCosts_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LandedCostAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LandedCostId = table.Column<int>(type: "int", nullable: false),
                    GoodsReceiptLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AllocationBasis = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CapitalizedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpensedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AverageUnitCostBefore = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    AverageUnitCostAfter = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandedCostAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandedCostAllocations_GoodsReceiptLines_GoodsReceiptLineId_TenantId",
                        columns: x => new { x.GoodsReceiptLineId, x.TenantId },
                        principalTable: "GoodsReceiptLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LandedCostAllocations_LandedCosts_LandedCostId_TenantId",
                        columns: x => new { x.LandedCostId, x.TenantId },
                        principalTable: "LandedCosts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandedCostAllocations_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LandedCostAllocations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LandedCostComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LandedCostId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandedCostComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandedCostComponents_LandedCosts_LandedCostId_TenantId",
                        columns: x => new { x.LandedCostId, x.TenantId },
                        principalTable: "LandedCosts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandedCostComponents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_LandedCostClearingAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "LandedCostClearingAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostAllocations_GoodsReceiptLineId_TenantId",
                table: "LandedCostAllocations",
                columns: new[] { "GoodsReceiptLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostAllocations_LandedCostId_TenantId",
                table: "LandedCostAllocations",
                columns: new[] { "LandedCostId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostAllocations_ProductId_TenantId",
                table: "LandedCostAllocations",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostAllocations_TenantId",
                table: "LandedCostAllocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostAllocations_TenantId_LandedCostId_GoodsReceiptLineId",
                table: "LandedCostAllocations",
                columns: new[] { "TenantId", "LandedCostId", "GoodsReceiptLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostComponents_LandedCostId_TenantId",
                table: "LandedCostComponents",
                columns: new[] { "LandedCostId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostComponents_TenantId",
                table: "LandedCostComponents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostComponents_TenantId_LandedCostId_Type",
                table: "LandedCostComponents",
                columns: new[] { "TenantId", "LandedCostId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandedCosts_GoodsReceiptId_TenantId",
                table: "LandedCosts",
                columns: new[] { "GoodsReceiptId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandedCosts_TenantId",
                table: "LandedCosts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LandedCosts_TenantId_CostNumber",
                table: "LandedCosts",
                columns: new[] { "TenantId", "CostNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandedCosts_TenantId_NormalizedExternalReference",
                table: "LandedCosts",
                columns: new[] { "TenantId", "NormalizedExternalReference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandedCosts_WarehouseId_TenantId",
                table: "LandedCosts",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_LandedCostClearingAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "LandedCostClearingAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_LandedCostClearingAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropTable(
                name: "LandedCostAllocations");

            migrationBuilder.DropTable(
                name: "LandedCostComponents");

            migrationBuilder.DropTable(
                name: "LandedCosts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_LandedCostClearingAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "LandedCostClearingAccountId",
                table: "AccountingSettings");
        }
    }
}
