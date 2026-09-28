using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConnectManagedUnitsToProductsAndRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeasurementUnitId",
                table: "RecipeIngredients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockMeasurementUnitId",
                table: "RecipeIngredients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StockUnitCodeSnapshot",
                table: "RecipeIngredients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "StockUnitFactorSnapshot",
                table: "RecipeIngredients",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UnitCodeSnapshot",
                table: "RecipeIngredients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitFactorSnapshot",
                table: "RecipeIngredients",
                type: "decimal(24,12)",
                precision: 24,
                scale: 12,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MeasurementUnitId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM sys.key_constraints
                    WHERE [name] = 'AK_MeasurementUnits_Id_TenantId'
                      AND [parent_object_id] = OBJECT_ID('MeasurementUnits'))
                BEGIN
                    ALTER TABLE MeasurementUnits
                    ADD CONSTRAINT AK_MeasurementUnits_Id_TenantId UNIQUE (Id, TenantId);
                END;

                INSERT INTO MeasurementUnits
                    (Code, Name, Symbol, Dimension, FactorToBaseUnit, DecimalPlaces, IsSystem, IsActive, TenantId)
                SELECT defaults.Code, defaults.Name, defaults.Symbol, defaults.Dimension,
                       defaults.FactorToBaseUnit, defaults.DecimalPlaces, 1, 1, tenants.Id
                FROM Tenants tenants
                CROSS JOIN (VALUES
                    ('PC', 'Piece', 'pc', 1, CAST(1 AS decimal(24,12)), 0),
                    ('MG', 'Milligram', 'mg', 2, CAST(0.001 AS decimal(24,12)), 6),
                    ('G', 'Gram', 'g', 2, CAST(1 AS decimal(24,12)), 3),
                    ('KG', 'Kilogram', 'kg', 2, CAST(1000 AS decimal(24,12)), 6),
                    ('OZ', 'Ounce', 'oz', 2, CAST(28.349523125 AS decimal(24,12)), 6),
                    ('LB', 'Pound', 'lb', 2, CAST(453.59237 AS decimal(24,12)), 6),
                    ('ML', 'Milliliter', 'ml', 3, CAST(1 AS decimal(24,12)), 3),
                    ('L', 'Liter', 'L', 3, CAST(1000 AS decimal(24,12)), 6)
                ) defaults(Code, Name, Symbol, Dimension, FactorToBaseUnit, DecimalPlaces)
                WHERE NOT EXISTS (
                    SELECT 1 FROM MeasurementUnits existing
                    WHERE existing.TenantId = tenants.Id AND existing.Code = defaults.Code);

                UPDATE products
                SET MeasurementUnitId = units.Id
                FROM Products products
                INNER JOIN MeasurementUnits units
                    ON units.TenantId = products.TenantId
                   AND units.Code = CASE products.StockUnit
                       WHEN 1 THEN 'PC' WHEN 2 THEN 'G' WHEN 3 THEN 'KG'
                       WHEN 4 THEN 'ML' WHEN 5 THEN 'L' END
                WHERE products.MeasurementUnitId IS NULL;

                UPDATE ingredients
                SET MeasurementUnitId = authored.Id,
                    StockMeasurementUnitId = stockUnit.Id,
                    UnitCodeSnapshot = authored.Code,
                    StockUnitCodeSnapshot = stockUnit.Code,
                    UnitFactorSnapshot = authored.FactorToBaseUnit,
                    StockUnitFactorSnapshot = stockUnit.FactorToBaseUnit
                FROM RecipeIngredients ingredients
                INNER JOIN MeasurementUnits authored
                    ON authored.TenantId = ingredients.TenantId
                   AND authored.Code = CASE ingredients.Unit
                       WHEN 1 THEN 'PC' WHEN 2 THEN 'G' WHEN 3 THEN 'KG'
                       WHEN 4 THEN 'ML' WHEN 5 THEN 'L' END
                INNER JOIN MeasurementUnits stockUnit
                    ON stockUnit.TenantId = ingredients.TenantId
                   AND stockUnit.Code = CASE ingredients.StockUnitSnapshot
                       WHEN 1 THEN 'PC' WHEN 2 THEN 'G' WHEN 3 THEN 'KG'
                       WHEN 4 THEN 'ML' WHEN 5 THEN 'L' END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_MeasurementUnitId_TenantId",
                table: "RecipeIngredients",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_StockMeasurementUnitId_TenantId",
                table: "RecipeIngredients",
                columns: new[] { "StockMeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_MeasurementUnitId_TenantId",
                table: "Products",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MeasurementUnits_MeasurementUnitId_TenantId",
                table: "Products",
                columns: new[] { "MeasurementUnitId", "TenantId" },
                principalTable: "MeasurementUnits",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_MeasurementUnits_MeasurementUnitId_TenantId",
                table: "RecipeIngredients",
                columns: new[] { "MeasurementUnitId", "TenantId" },
                principalTable: "MeasurementUnits",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_MeasurementUnits_StockMeasurementUnitId_TenantId",
                table: "RecipeIngredients",
                columns: new[] { "StockMeasurementUnitId", "TenantId" },
                principalTable: "MeasurementUnits",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MeasurementUnits_MeasurementUnitId_TenantId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_MeasurementUnits_MeasurementUnitId_TenantId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_MeasurementUnits_StockMeasurementUnitId_TenantId",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_MeasurementUnitId_TenantId",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_StockMeasurementUnitId_TenantId",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_Products_MeasurementUnitId_TenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "StockMeasurementUnitId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "StockUnitCodeSnapshot",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "StockUnitFactorSnapshot",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "UnitCodeSnapshot",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "UnitFactorSnapshot",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "Products");
        }
    }
}
