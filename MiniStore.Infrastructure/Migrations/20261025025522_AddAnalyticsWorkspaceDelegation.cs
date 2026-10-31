using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsWorkspaceDelegation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DelegationRevision",
                table: "AnalyticsWorkspaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AnalyticsWorkspaceDelegations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalyticsWorkspaceId = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    TargetRoleId = table.Column<int>(type: "int", nullable: true),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsWorkspaceDelegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalyticsWorkspaceDelegations_AnalyticsWorkspaces_AnalyticsWorkspaceId_TenantId",
                        columns: x => new { x.AnalyticsWorkspaceId, x.TenantId },
                        principalTable: "AnalyticsWorkspaces",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalyticsWorkspaceDelegations_TenantRoles_TargetRoleId_TenantId",
                        columns: x => new { x.TargetRoleId, x.TenantId },
                        principalTable: "TenantRoles",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnalyticsWorkspaceDelegations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_AnalyticsWorkspaceId_TargetType_TargetRoleId",
                table: "AnalyticsWorkspaceDelegations",
                columns: new[] { "AnalyticsWorkspaceId", "TargetType", "TargetRoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_AnalyticsWorkspaceId_TargetType_TargetUserId",
                table: "AnalyticsWorkspaceDelegations",
                columns: new[] { "AnalyticsWorkspaceId", "TargetType", "TargetUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_AnalyticsWorkspaceId_TenantId",
                table: "AnalyticsWorkspaceDelegations",
                columns: new[] { "AnalyticsWorkspaceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_TargetRoleId_RevokedAtUtc_ExpiresAtUtc",
                table: "AnalyticsWorkspaceDelegations",
                columns: new[] { "TargetRoleId", "RevokedAtUtc", "ExpiresAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_TargetRoleId_TenantId",
                table: "AnalyticsWorkspaceDelegations",
                columns: new[] { "TargetRoleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_TargetUserId_RevokedAtUtc_ExpiresAtUtc",
                table: "AnalyticsWorkspaceDelegations",
                columns: new[] { "TargetUserId", "RevokedAtUtc", "ExpiresAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsWorkspaceDelegations_TenantId",
                table: "AnalyticsWorkspaceDelegations",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsWorkspaceDelegations");

            migrationBuilder.DropColumn(
                name: "DelegationRevision",
                table: "AnalyticsWorkspaces");
        }
    }
}
