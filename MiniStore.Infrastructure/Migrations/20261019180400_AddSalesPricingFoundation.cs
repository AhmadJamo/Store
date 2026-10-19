using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesPricingFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultSalesPriceListId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalesPriceLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    RoundingIncrement = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPriceLists", x => x.Id);
                    table.UniqueConstraint("AK_SalesPriceLists_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SalesPriceLists_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesPriceRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesPriceListId = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: true),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: true),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    ComputationType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPriceRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesPriceRules_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPriceRules_ProductCategories_ProductCategoryId_TenantId",
                        columns: x => new { x.ProductCategoryId, x.TenantId },
                        principalTable: "ProductCategories",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPriceRules_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPriceRules_SalesPriceLists_SalesPriceListId_TenantId",
                        columns: x => new { x.SalesPriceListId, x.TenantId },
                        principalTable: "SalesPriceLists",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPriceRules_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DefaultSalesPriceListId_TenantId",
                table: "Customers",
                columns: new[] { "DefaultSalesPriceListId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceLists_IsActive_Name",
                table: "SalesPriceLists",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceLists_TenantId",
                table: "SalesPriceLists",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceLists_TenantId_Code",
                table: "SalesPriceLists",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_MeasurementUnitId_TenantId",
                table: "SalesPriceRules",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_ProductCategoryId_TenantId",
                table: "SalesPriceRules",
                columns: new[] { "ProductCategoryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_ProductId_TenantId",
                table: "SalesPriceRules",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_SalesPriceListId_IsActive_Priority",
                table: "SalesPriceRules",
                columns: new[] { "SalesPriceListId", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_SalesPriceListId_TenantId",
                table: "SalesPriceRules",
                columns: new[] { "SalesPriceListId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_TenantId",
                table: "SalesPriceRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_TenantId_SalesPriceListId_Scope_MeasurementUnitId_MinimumQuantity_Priority_EffectiveFrom",
                table: "SalesPriceRules",
                columns: new[] { "TenantId", "SalesPriceListId", "Scope", "MeasurementUnitId", "MinimumQuantity", "Priority", "EffectiveFrom" },
                unique: true,
                filter: "[Scope] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_TenantId_SalesPriceListId_Scope_ProductCategoryId_MeasurementUnitId_MinimumQuantity_Priority_EffectiveFrom",
                table: "SalesPriceRules",
                columns: new[] { "TenantId", "SalesPriceListId", "Scope", "ProductCategoryId", "MeasurementUnitId", "MinimumQuantity", "Priority", "EffectiveFrom" },
                unique: true,
                filter: "[Scope] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPriceRules_TenantId_SalesPriceListId_Scope_ProductId_MeasurementUnitId_MinimumQuantity_Priority_EffectiveFrom",
                table: "SalesPriceRules",
                columns: new[] { "TenantId", "SalesPriceListId", "Scope", "ProductId", "MeasurementUnitId", "MinimumQuantity", "Priority", "EffectiveFrom" },
                unique: true,
                filter: "[Scope] = 3");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_SalesPriceLists_DefaultSalesPriceListId_TenantId",
                table: "Customers",
                columns: new[] { "DefaultSalesPriceListId", "TenantId" },
                principalTable: "SalesPriceLists",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_SalesPriceLists_DefaultSalesPriceListId_TenantId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "SalesPriceRules");

            migrationBuilder.DropTable(
                name: "SalesPriceLists");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DefaultSalesPriceListId_TenantId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultSalesPriceListId",
                table: "Customers");
        }
    }
}
