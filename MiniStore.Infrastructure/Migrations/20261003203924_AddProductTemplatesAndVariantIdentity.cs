using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTemplatesAndVariantIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTemplateId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantLabel",
                table: "Products",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantSignature",
                table: "Products",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTemplates", x => x.Id);
                    table.UniqueConstraint("AK_ProductTemplates_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_ProductTemplates_ProductCategories_ProductCategoryId_TenantId",
                        columns: x => new { x.ProductCategoryId, x.TenantId },
                        principalTable: "ProductCategories",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTemplates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTemplateId_TenantId",
                table: "Products",
                columns: new[] { "ProductTemplateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_ProductTemplateId_VariantSignature",
                table: "Products",
                columns: new[] { "TenantId", "ProductTemplateId", "VariantSignature" },
                unique: true,
                filter: "[ProductTemplateId] IS NOT NULL AND [VariantSignature] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_ProductCategoryId_TenantId",
                table: "ProductTemplates",
                columns: new[] { "ProductCategoryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_TenantId",
                table: "ProductTemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_TenantId_Code",
                table: "ProductTemplates",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductTemplates_ProductTemplateId_TenantId",
                table: "Products",
                columns: new[] { "ProductTemplateId", "TenantId" },
                principalTable: "ProductTemplates",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductTemplates_ProductTemplateId_TenantId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductTemplateId_TenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_ProductTemplateId_VariantSignature",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductTemplateId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VariantLabel",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VariantSignature",
                table: "Products");
        }
    }
}
