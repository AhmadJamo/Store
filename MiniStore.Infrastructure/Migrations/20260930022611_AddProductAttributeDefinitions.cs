using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAttributeDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductAttributeDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVariantDefining = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeDefinitions", x => x.Id);
                    table.UniqueConstraint("AK_ProductAttributeDefinitions_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_ProductAttributeDefinitions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductAttributeDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeOptions_ProductAttributeDefinitions_ProductAttributeDefinitionId_TenantId",
                        columns: x => new { x.ProductAttributeDefinitionId, x.TenantId },
                        principalTable: "ProductAttributeDefinitions",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeOptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategoryAttributes",
                columns: table => new
                {
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeDefinitionId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryAttributes", x => new { x.ProductCategoryId, x.ProductAttributeDefinitionId });
                    table.ForeignKey(
                        name: "FK_ProductCategoryAttributes_ProductAttributeDefinitions_ProductAttributeDefinitionId_TenantId",
                        columns: x => new { x.ProductAttributeDefinitionId, x.TenantId },
                        principalTable: "ProductAttributeDefinitions",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryAttributes_ProductCategories_ProductCategoryId_TenantId",
                        columns: x => new { x.ProductCategoryId, x.TenantId },
                        principalTable: "ProductCategories",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryAttributes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeDefinitions_TenantId",
                table: "ProductAttributeDefinitions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeDefinitions_TenantId_Code",
                table: "ProductAttributeDefinitions",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeOptions_ProductAttributeDefinitionId_TenantId",
                table: "ProductAttributeOptions",
                columns: new[] { "ProductAttributeDefinitionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeOptions_TenantId",
                table: "ProductAttributeOptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeOptions_TenantId_ProductAttributeDefinitionId_Code",
                table: "ProductAttributeOptions",
                columns: new[] { "TenantId", "ProductAttributeDefinitionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryAttributes_ProductAttributeDefinitionId_TenantId",
                table: "ProductCategoryAttributes",
                columns: new[] { "ProductAttributeDefinitionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryAttributes_ProductCategoryId_TenantId",
                table: "ProductCategoryAttributes",
                columns: new[] { "ProductCategoryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryAttributes_TenantId",
                table: "ProductCategoryAttributes",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttributeOptions");

            migrationBuilder.DropTable(
                name: "ProductCategoryAttributes");

            migrationBuilder.DropTable(
                name: "ProductAttributeDefinitions");
        }
    }
}
