using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCafeRecipesAndControlledNegativeStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockTransactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AddColumn<int>(
                name: "ProductRecipeId",
                table: "SaleItems",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductStocks",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AddColumn<bool>(
                name: "AllowNegativeRecipeConsumption",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InventoryBehavior",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "StockUnit",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "ProductRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    YieldQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecipes", x => x.Id);
                    table.UniqueConstraint("AK_ProductRecipes_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_ProductRecipes_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRecipes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductRecipeId = table.Column<int>(type: "int", nullable: false),
                    IngredientProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    StockUnitSnapshot = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_ProductRecipes_ProductRecipeId_TenantId",
                        columns: x => new { x.ProductRecipeId, x.TenantId },
                        principalTable: "ProductRecipes",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Products_IngredientProductId_TenantId",
                        columns: x => new { x.IngredientProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductRecipeId_TenantId",
                table: "SaleItems",
                columns: new[] { "ProductRecipeId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecipes_ProductId_TenantId",
                table: "ProductRecipes",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecipes_TenantId",
                table: "ProductRecipes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecipes_TenantId_ProductId_IsActive",
                table: "ProductRecipes",
                columns: new[] { "TenantId", "ProductId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecipes_TenantId_ProductId_VersionNumber",
                table: "ProductRecipes",
                columns: new[] { "TenantId", "ProductId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientProductId_TenantId",
                table: "RecipeIngredients",
                columns: new[] { "IngredientProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_ProductRecipeId_TenantId",
                table: "RecipeIngredients",
                columns: new[] { "ProductRecipeId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_TenantId",
                table: "RecipeIngredients",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_TenantId_ProductRecipeId_IngredientProductId",
                table: "RecipeIngredients",
                columns: new[] { "TenantId", "ProductRecipeId", "IngredientProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_ProductRecipes_ProductRecipeId_TenantId",
                table: "SaleItems",
                columns: new[] { "ProductRecipeId", "TenantId" },
                principalTable: "ProductRecipes",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_ProductRecipes_ProductRecipeId_TenantId",
                table: "SaleItems");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "ProductRecipes");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_ProductRecipeId_TenantId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "ProductRecipeId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "AllowNegativeRecipeConsumption",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InventoryBehavior",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StockUnit",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockTransactions",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductStocks",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);
        }
    }
}
