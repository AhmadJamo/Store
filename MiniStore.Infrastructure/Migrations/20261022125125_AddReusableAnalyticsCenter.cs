using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReusableAnalyticsCenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalyticsWorkspaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameArabic = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ModuleKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    OwnerUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    LayoutColumns = table.Column<int>(type: "int", nullable: false),
                    DefaultPeriodDays = table.Column<int>(type: "int", nullable: false),
                    ComparisonMode = table.Column<int>(type: "int", nullable: false),
                    RefreshIntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    ShowDefinitions = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsWorkspaces", x => x.Id);
                    table.UniqueConstraint("AK_AnalyticsWorkspaces_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_AnalyticsWorkspaces_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnalyticsWorkspaceWidgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalyticsWorkspaceId = table.Column<int>(type: "int", nullable: false),
                    WidgetKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Visualization = table.Column<int>(type: "int", nullable: false),
                    TitleOverride = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TitleArabicOverride = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ColumnSpan = table.Column<int>(type: "int", nullable: false),
                    RowSpan = table.Column<int>(type: "int", nullable: false),
                    TopN = table.Column<int>(type: "int", nullable: false),
                    ShowLegend = table.Column<bool>(type: "bit", nullable: false),
                    ShowComparison = table.Column<bool>(type: "bit", nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    AccentColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsWorkspaceWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalyticsWorkspaceWidgets_AnalyticsWorkspaces_AnalyticsWorkspaceId_TenantId",
                        columns: x => new { x.AnalyticsWorkspaceId, x.TenantId },
                        principalTable: "AnalyticsWorkspaces",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalyticsWorkspaceWidgets_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaces_ModuleKey_Visibility",
                table: "AnalyticsWorkspaces",
                columns: new[] { "ModuleKey", "Visibility" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaces_OwnerUserId_ModuleKey",
                table: "AnalyticsWorkspaces",
                columns: new[] { "OwnerUserId", "ModuleKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaces_TenantId",
                table: "AnalyticsWorkspaces",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceWidgets_AnalyticsWorkspaceId_TenantId",
                table: "AnalyticsWorkspaceWidgets",
                columns: new[] { "AnalyticsWorkspaceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceWidgets_TenantId",
                table: "AnalyticsWorkspaceWidgets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceWidgets_TenantId_AnalyticsWorkspaceId_Sequence",
                table: "AnalyticsWorkspaceWidgets",
                columns: new[] { "TenantId", "AnalyticsWorkspaceId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceWidgets_TenantId_AnalyticsWorkspaceId_WidgetKey",
                table: "AnalyticsWorkspaceWidgets",
                columns: new[] { "TenantId", "AnalyticsWorkspaceId", "WidgetKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsWorkspaceWidgets");

            migrationBuilder.DropTable(
                name: "AnalyticsWorkspaces");
        }
    }
}
