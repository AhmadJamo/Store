using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedQuotationTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FooterNotesSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntroductionSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesQuotationTemplateId",
                table: "SalesQuotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateCodeSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateNameSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemplateVersionSnapshot",
                table: "SalesQuotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditionsSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowQuantityChangeSnapshot",
                table: "SalesQuotationLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SalesQuotationTemplateItemId",
                table: "SalesQuotationLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemplateItemType",
                table: "SalesQuotationLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateOptionGroupSnapshot",
                table: "SalesQuotationLines",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateSectionSnapshot",
                table: "SalesQuotationLines",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalesQuotationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    SourceTemplateId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidityDays = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    SalesPriceListId = table.Column<int>(type: "int", nullable: true),
                    AllowAdditionalLines = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    FooterNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesQuotationTemplates", x => x.Id);
                    table.UniqueConstraint("AK_SalesQuotationTemplates_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_SalesQuotationTemplates_VersionValidity", "[Version] BETWEEN 1 AND 9999 AND [ValidityDays] BETWEEN 0 AND 3650");
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplates_SalesPriceLists_SalesPriceListId_TenantId",
                        columns: x => new { x.SalesPriceListId, x.TenantId },
                        principalTable: "SalesPriceLists",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplates_SalesQuotationTemplates_SourceTemplateId_TenantId",
                        columns: x => new { x.SourceTemplateId, x.TenantId },
                        principalTable: "SalesQuotationTemplates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesQuotationTemplateItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesQuotationTemplateId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    SectionTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultQuantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    MaximumQuantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    AllowQuantityChange = table.Column<bool>(type: "bit", nullable: false),
                    TaxRateId = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    IsDefaultSelected = table.Column<bool>(type: "bit", nullable: false),
                    OptionGroupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OptionGroupName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    GroupMinimumSelections = table.Column<int>(type: "int", nullable: false),
                    GroupMaximumSelections = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesQuotationTemplateItems", x => x.Id);
                    table.UniqueConstraint("AK_SalesQuotationTemplateItems_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_SalesQuotationTemplateItems_Quantity", "[DefaultQuantity] > 0 AND [MinimumQuantity] > 0 AND [MaximumQuantity] >= [MinimumQuantity] AND [DefaultQuantity] BETWEEN [MinimumQuantity] AND [MaximumQuantity]");
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplateItems_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplateItems_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplateItems_SalesQuotationTemplates_SalesQuotationTemplateId_TenantId",
                        columns: x => new { x.SalesQuotationTemplateId, x.TenantId },
                        principalTable: "SalesQuotationTemplates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplateItems_TaxRates_TaxRateId_TenantId",
                        columns: x => new { x.TaxRateId, x.TenantId },
                        principalTable: "TaxRates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesQuotationTemplateItems_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_SalesQuotationTemplateId_TenantId",
                table: "SalesQuotations",
                columns: new[] { "SalesQuotationTemplateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_SalesQuotationTemplateItemId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "SalesQuotationTemplateItemId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplateItems_MeasurementUnitId_TenantId",
                table: "SalesQuotationTemplateItems",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplateItems_ProductId_TenantId",
                table: "SalesQuotationTemplateItems",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplateItems_SalesQuotationTemplateId_TenantId",
                table: "SalesQuotationTemplateItems",
                columns: new[] { "SalesQuotationTemplateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplateItems_TaxRateId_TenantId",
                table: "SalesQuotationTemplateItems",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplateItems_TenantId",
                table: "SalesQuotationTemplateItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplateItems_TenantId_SalesQuotationTemplateId_Sequence",
                table: "SalesQuotationTemplateItems",
                columns: new[] { "TenantId", "SalesQuotationTemplateId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_SalesPriceListId_TenantId",
                table: "SalesQuotationTemplates",
                columns: new[] { "SalesPriceListId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_SourceTemplateId_TenantId",
                table: "SalesQuotationTemplates",
                columns: new[] { "SourceTemplateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_Status_Category",
                table: "SalesQuotationTemplates",
                columns: new[] { "Status", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_TenantId",
                table: "SalesQuotationTemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_TenantId_Code_Version",
                table: "SalesQuotationTemplates",
                columns: new[] { "TenantId", "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_TenantId_IsDefault",
                table: "SalesQuotationTemplates",
                columns: new[] { "TenantId", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1 AND [Status] = 2");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesQuotationLines_SalesQuotationTemplateItems_SalesQuotationTemplateItemId_TenantId",
                table: "SalesQuotationLines",
                columns: new[] { "SalesQuotationTemplateItemId", "TenantId" },
                principalTable: "SalesQuotationTemplateItems",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesQuotations_SalesQuotationTemplates_SalesQuotationTemplateId_TenantId",
                table: "SalesQuotations",
                columns: new[] { "SalesQuotationTemplateId", "TenantId" },
                principalTable: "SalesQuotationTemplates",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesQuotationLines_SalesQuotationTemplateItems_SalesQuotationTemplateItemId_TenantId",
                table: "SalesQuotationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesQuotations_SalesQuotationTemplates_SalesQuotationTemplateId_TenantId",
                table: "SalesQuotations");

            migrationBuilder.DropTable(
                name: "SalesQuotationTemplateItems");

            migrationBuilder.DropTable(
                name: "SalesQuotationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_SalesQuotations_SalesQuotationTemplateId_TenantId",
                table: "SalesQuotations");

            migrationBuilder.DropIndex(
                name: "IX_SalesQuotationLines_SalesQuotationTemplateItemId_TenantId",
                table: "SalesQuotationLines");

            migrationBuilder.DropColumn(
                name: "FooterNotesSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "IntroductionSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "SalesQuotationTemplateId",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "TemplateCodeSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "TemplateNameSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "TemplateVersionSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "TermsAndConditionsSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "AllowQuantityChangeSnapshot",
                table: "SalesQuotationLines");

            migrationBuilder.DropColumn(
                name: "SalesQuotationTemplateItemId",
                table: "SalesQuotationLines");

            migrationBuilder.DropColumn(
                name: "TemplateItemType",
                table: "SalesQuotationLines");

            migrationBuilder.DropColumn(
                name: "TemplateOptionGroupSnapshot",
                table: "SalesQuotationLines");

            migrationBuilder.DropColumn(
                name: "TemplateSectionSnapshot",
                table: "SalesQuotationLines");
        }
    }
}
