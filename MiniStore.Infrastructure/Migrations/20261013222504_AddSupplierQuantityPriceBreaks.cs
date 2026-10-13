using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierQuantityPriceBreaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceBreakRevision",
                table: "SupplierProductPurchasingInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SupplierProductPurchasingInfos_Id_TenantId",
                table: "SupplierProductPurchasingInfos",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "SupplierProductPriceBreaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierProductPurchasingInfoId = table.Column<int>(type: "int", nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProductPriceBreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceBreaks_SupplierProductPurchasingInfos_SupplierProductPurchasingInfoId_TenantId",
                        columns: x => new { x.SupplierProductPurchasingInfoId, x.TenantId },
                        principalTable: "SupplierProductPurchasingInfos",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierProductPriceBreaks_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceBreaks_SupplierProductPurchasingInfoId_TenantId",
                table: "SupplierProductPriceBreaks",
                columns: new[] { "SupplierProductPurchasingInfoId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceBreaks_TenantId",
                table: "SupplierProductPriceBreaks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPriceBreaks_TenantId_SupplierProductPurchasingInfoId_MinimumQuantity",
                table: "SupplierProductPriceBreaks",
                columns: new[] { "TenantId", "SupplierProductPurchasingInfoId", "MinimumQuantity" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierProductPriceBreaks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SupplierProductPurchasingInfos_Id_TenantId",
                table: "SupplierProductPurchasingInfos");

            migrationBuilder.DropColumn(
                name: "PriceBreakRevision",
                table: "SupplierProductPurchasingInfos");
        }
    }
}
