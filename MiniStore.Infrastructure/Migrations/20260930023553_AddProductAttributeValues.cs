using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAttributeValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProductAttributeOptions_Id_TenantId",
                table: "ProductAttributeOptions",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeDefinitionId = table.Column<int>(type: "int", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumberValue = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    BooleanValue = table.Column<bool>(type: "bit", nullable: true),
                    ProductAttributeOptionId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValues", x => x.Id);
                    table.CheckConstraint("CK_ProductAttributeValues_ExactlyOneValue", "(CASE WHEN [TextValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [NumberValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [BooleanValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ProductAttributeOptionId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_ProductAttributeDefinitions_ProductAttributeDefinitionId_TenantId",
                        columns: x => new { x.ProductAttributeDefinitionId, x.TenantId },
                        principalTable: "ProductAttributeDefinitions",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_ProductAttributeOptions_ProductAttributeOptionId_TenantId",
                        columns: x => new { x.ProductAttributeOptionId, x.TenantId },
                        principalTable: "ProductAttributeOptions",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductAttributeDefinitionId_TenantId",
                table: "ProductAttributeValues",
                columns: new[] { "ProductAttributeDefinitionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductAttributeOptionId_TenantId",
                table: "ProductAttributeValues",
                columns: new[] { "ProductAttributeOptionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductId_TenantId",
                table: "ProductAttributeValues",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_TenantId",
                table: "ProductAttributeValues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_TenantId_ProductId_ProductAttributeDefinitionId",
                table: "ProductAttributeValues",
                columns: new[] { "TenantId", "ProductId", "ProductAttributeDefinitionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttributeValues");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProductAttributeOptions_Id_TenantId",
                table: "ProductAttributeOptions");
        }
    }
}
