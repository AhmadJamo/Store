using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationIdentityAndDocumentDesigner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentDesignId",
                table: "SalesQuotationTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignCodeSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesignLanguageSnapshot",
                table: "SalesQuotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignNameSnapshot",
                table: "SalesQuotations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesignVersionSnapshot",
                table: "SalesQuotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentDesignId",
                table: "SalesQuotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressArabic",
                table: "GeneralSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "GeneralSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityArabic",
                table: "GeneralSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegistrationNumber",
                table: "GeneralSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "GeneralSettings",
                type: "nchar(2)",
                fixedLength: true,
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentNameStyle",
                table: "GeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "GeneralSettings",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalName",
                table: "GeneralSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalNameArabic",
                table: "GeneralSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "GeneralSettings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhone",
                table: "GeneralSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateOrProvince",
                table: "GeneralSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxRegistrationNumber",
                table: "GeneralSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "GeneralSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GeneralSettings_Id_TenantId",
                table: "GeneralSettings",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "DocumentDesigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    SourceDesignId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultLanguage = table.Column<int>(type: "int", nullable: false),
                    AllowLanguageOverride = table.Column<bool>(type: "bit", nullable: false),
                    PageSize = table.Column<int>(type: "int", nullable: false),
                    Orientation = table.Column<int>(type: "int", nullable: false),
                    CustomPageWidthMm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    CustomPageHeightMm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    MarginTopMm = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MarginEndMm = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MarginBottomMm = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MarginStartMm = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PrimaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    TextColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    TableHeaderTextColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    FontFamily = table.Column<int>(type: "int", nullable: false),
                    HeaderLayout = table.Column<int>(type: "int", nullable: false),
                    UseOrganizationLogo = table.Column<bool>(type: "bit", nullable: false),
                    LogoPlacement = table.Column<int>(type: "int", nullable: false),
                    LogoMaximumHeightPx = table.Column<int>(type: "int", nullable: false),
                    OrganizationNamePlacement = table.Column<int>(type: "int", nullable: false),
                    ShowLegalName = table.Column<bool>(type: "bit", nullable: false),
                    ShowTradeName = table.Column<bool>(type: "bit", nullable: false),
                    ShowRegistrationNumber = table.Column<bool>(type: "bit", nullable: false),
                    ShowTaxNumber = table.Column<bool>(type: "bit", nullable: false),
                    ShowAddress = table.Column<bool>(type: "bit", nullable: false),
                    ShowContactDetails = table.Column<bool>(type: "bit", nullable: false),
                    ShowCustomIdentityFields = table.Column<bool>(type: "bit", nullable: false),
                    ShowPageNumbers = table.Column<bool>(type: "bit", nullable: false),
                    ShowStatusWatermark = table.Column<bool>(type: "bit", nullable: false),
                    ShowProductCode = table.Column<bool>(type: "bit", nullable: false),
                    ShowUnit = table.Column<bool>(type: "bit", nullable: false),
                    ShowLineNotes = table.Column<bool>(type: "bit", nullable: false),
                    ShowTaxDetails = table.Column<bool>(type: "bit", nullable: false),
                    ShowNetTotal = table.Column<bool>(type: "bit", nullable: false),
                    ShowTaxTotal = table.Column<bool>(type: "bit", nullable: false),
                    ShowQrCode = table.Column<bool>(type: "bit", nullable: false),
                    ShowDocumentBarcode = table.Column<bool>(type: "bit", nullable: false),
                    CoverTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CoverTitleArabic = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CoverSubtitle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CoverSubtitleArabic = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HeaderText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HeaderTextArabic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FooterText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FooterTextArabic = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WatermarkText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WatermarkTextArabic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WatermarkOpacity = table.Column<decimal>(type: "decimal(4,3)", precision: 4, scale: 3, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDesigns", x => x.Id);
                    table.UniqueConstraint("AK_DocumentDesigns_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_DocumentDesigns_LogoHeight", "[LogoMaximumHeightPx] BETWEEN 24 AND 300");
                    table.CheckConstraint("CK_DocumentDesigns_Margins", "[MarginTopMm] BETWEEN 0 AND 50 AND [MarginEndMm] BETWEEN 0 AND 50 AND [MarginBottomMm] BETWEEN 0 AND 50 AND [MarginStartMm] BETWEEN 0 AND 50");
                    table.CheckConstraint("CK_DocumentDesigns_Version", "[Version] BETWEEN 1 AND 9999");
                    table.CheckConstraint("CK_DocumentDesigns_WatermarkOpacity", "[WatermarkOpacity] BETWEEN 0.02 AND 0.5");
                    table.ForeignKey(
                        name: "FK_DocumentDesigns_DocumentDesigns_SourceDesignId_TenantId",
                        columns: x => new { x.SourceDesignId, x.TenantId },
                        principalTable: "DocumentDesigns",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentDesigns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationIdentityFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneralSettingsId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LabelArabic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ValueArabic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentScope = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationIdentityFields", x => x.Id);
                    table.CheckConstraint("CK_OrganizationIdentityFields_DocumentScope", "[DocumentScope] BETWEEN 0 AND 15");
                    table.CheckConstraint("CK_OrganizationIdentityFields_Sequence", "[Sequence] BETWEEN 1 AND 100");
                    table.ForeignKey(
                        name: "FK_OrganizationIdentityFields_GeneralSettings_GeneralSettingsId_TenantId",
                        columns: x => new { x.GeneralSettingsId, x.TenantId },
                        principalTable: "GeneralSettings",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationIdentityFields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationLogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneralSettingsId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationLogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationLogos_GeneralSettings_GeneralSettingsId_TenantId",
                        columns: x => new { x.GeneralSettingsId, x.TenantId },
                        principalTable: "GeneralSettings",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationLogos_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentDesignSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDesignId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    SectionType = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    PageBreakBefore = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TitleArabic = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContentArabic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDesignSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentDesignSections_DocumentDesigns_DocumentDesignId_TenantId",
                        columns: x => new { x.DocumentDesignId, x.TenantId },
                        principalTable: "DocumentDesigns",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentDesignSections_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationTemplates_DocumentDesignId_TenantId",
                table: "SalesQuotationTemplates",
                columns: new[] { "DocumentDesignId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotations_DocumentDesignId_TenantId",
                table: "SalesQuotations",
                columns: new[] { "DocumentDesignId", "TenantId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_GeneralSettings_DocumentNameStyle",
                table: "GeneralSettings",
                sql: "[DocumentNameStyle] BETWEEN 1 AND 3");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesigns_SourceDesignId_TenantId",
                table: "DocumentDesigns",
                columns: new[] { "SourceDesignId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesigns_TenantId",
                table: "DocumentDesigns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesigns_TenantId_DocumentType_Code_Version",
                table: "DocumentDesigns",
                columns: new[] { "TenantId", "DocumentType", "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesigns_TenantId_DocumentType_IsDefault",
                table: "DocumentDesigns",
                columns: new[] { "TenantId", "DocumentType", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1 AND [Status] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesignSections_DocumentDesignId_TenantId",
                table: "DocumentDesignSections",
                columns: new[] { "DocumentDesignId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesignSections_TenantId",
                table: "DocumentDesignSections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDesignSections_TenantId_DocumentDesignId_Sequence",
                table: "DocumentDesignSections",
                columns: new[] { "TenantId", "DocumentDesignId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentityFields_GeneralSettingsId_TenantId",
                table: "OrganizationIdentityFields",
                columns: new[] { "GeneralSettingsId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentityFields_TenantId",
                table: "OrganizationIdentityFields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentityFields_TenantId_GeneralSettingsId_Sequence",
                table: "OrganizationIdentityFields",
                columns: new[] { "TenantId", "GeneralSettingsId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationLogos_GeneralSettingsId_TenantId",
                table: "OrganizationLogos",
                columns: new[] { "GeneralSettingsId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationLogos_TenantId",
                table: "OrganizationLogos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationLogos_TenantId_GeneralSettingsId",
                table: "OrganizationLogos",
                columns: new[] { "TenantId", "GeneralSettingsId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesQuotations_DocumentDesigns_DocumentDesignId_TenantId",
                table: "SalesQuotations",
                columns: new[] { "DocumentDesignId", "TenantId" },
                principalTable: "DocumentDesigns",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesQuotationTemplates_DocumentDesigns_DocumentDesignId_TenantId",
                table: "SalesQuotationTemplates",
                columns: new[] { "DocumentDesignId", "TenantId" },
                principalTable: "DocumentDesigns",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesQuotations_DocumentDesigns_DocumentDesignId_TenantId",
                table: "SalesQuotations");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesQuotationTemplates_DocumentDesigns_DocumentDesignId_TenantId",
                table: "SalesQuotationTemplates");

            migrationBuilder.DropTable(
                name: "DocumentDesignSections");

            migrationBuilder.DropTable(
                name: "OrganizationIdentityFields");

            migrationBuilder.DropTable(
                name: "OrganizationLogos");

            migrationBuilder.DropTable(
                name: "DocumentDesigns");

            migrationBuilder.DropIndex(
                name: "IX_SalesQuotationTemplates_DocumentDesignId_TenantId",
                table: "SalesQuotationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_SalesQuotations_DocumentDesignId_TenantId",
                table: "SalesQuotations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GeneralSettings_Id_TenantId",
                table: "GeneralSettings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GeneralSettings_DocumentNameStyle",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "DocumentDesignId",
                table: "SalesQuotationTemplates");

            migrationBuilder.DropColumn(
                name: "DesignCodeSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "DesignLanguageSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "DesignNameSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "DesignVersionSnapshot",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "DocumentDesignId",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "AddressArabic",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "City",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "CityArabic",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "CommercialRegistrationNumber",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "DocumentNameStyle",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "LegalName",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "LegalNameArabic",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "SecondaryPhone",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "StateOrProvince",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "TaxRegistrationNumber",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "GeneralSettings");
        }
    }
}
