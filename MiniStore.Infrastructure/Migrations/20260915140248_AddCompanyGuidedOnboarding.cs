using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyGuidedOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyOnboardings",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BusinessType = table.Column<int>(type: "int", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Language = table.Column<int>(type: "int", nullable: true),
                    FiscalYearStartMonth = table.Column<int>(type: "int", nullable: true),
                    IsTaxRegistered = table.Column<bool>(type: "bit", nullable: true),
                    DefaultTaxRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    UsePos = table.Column<bool>(type: "bit", nullable: true),
                    InventoryControlMode = table.Column<int>(type: "int", nullable: true),
                    TemplateVersion = table.Column<int>(type: "int", nullable: true),
                    CompletedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyOnboardings", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_CompanyOnboardings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Existing development companies predate the wizard; preserve their manual setup.
            migrationBuilder.Sql(
                """
                INSERT INTO [CompanyOnboardings] ([TenantId], [Status], [CompletedAtUtc])
                SELECT [Id], 3, SYSUTCDATETIME()
                FROM [Tenants];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyOnboardings");
        }
    }
}
