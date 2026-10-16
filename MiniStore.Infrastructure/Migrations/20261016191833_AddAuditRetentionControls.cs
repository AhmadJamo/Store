using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditRetentionControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLegalHolds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    FromUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReasonRedacted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ReleasedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReleaseReasonRedacted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReleasedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLegalHolds", x => x.Id);
                    table.CheckConstraint("CK_AuditLegalHolds_Range", "[ToUtc] >= [FromUtc]");
                    table.CheckConstraint("CK_AuditLegalHolds_ScopeTenant", "([Scope] = 1 AND [TenantId] IS NOT NULL AND [TenantId] > 0) OR ([Scope] IN (2, 3) AND [TenantId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_AuditLegalHolds_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLegalHolds_HoldId",
                table: "AuditLegalHolds",
                column: "HoldId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLegalHolds_IsActive_Scope_TenantId_FromUtc_ToUtc",
                table: "AuditLegalHolds",
                columns: new[] { "IsActive", "Scope", "TenantId", "FromUtc", "ToUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLegalHolds_TenantId",
                table: "AuditLegalHolds",
                column: "TenantId");

            migrationBuilder.Sql("""
                IF DATABASE_PRINCIPAL_ID(N'MiniStoreAuditWriter') IS NULL CREATE ROLE [MiniStoreAuditWriter];
                IF DATABASE_PRINCIPAL_ID(N'MiniStoreAuditReader') IS NULL CREATE ROLE [MiniStoreAuditReader];
                IF DATABASE_PRINCIPAL_ID(N'MiniStoreAuditMaintainer') IS NULL CREATE ROLE [MiniStoreAuditMaintainer];

                GRANT INSERT ON [dbo].[AuditEvents] TO [MiniStoreAuditWriter];
                DENY UPDATE, DELETE ON [dbo].[AuditEvents] TO [MiniStoreAuditWriter];

                GRANT SELECT ON [dbo].[AuditEvents] TO [MiniStoreAuditReader];
                DENY INSERT, UPDATE, DELETE ON [dbo].[AuditEvents] TO [MiniStoreAuditReader];

                GRANT SELECT, DELETE ON [dbo].[AuditEvents] TO [MiniStoreAuditMaintainer];
                DENY INSERT, UPDATE ON [dbo].[AuditEvents] TO [MiniStoreAuditMaintainer];
                GRANT SELECT, INSERT, UPDATE ON [dbo].[AuditLegalHolds] TO [MiniStoreAuditMaintainer];
                DENY DELETE ON [dbo].[AuditLegalHolds] TO [MiniStoreAuditMaintainer];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLegalHolds");

            migrationBuilder.Sql("""
                IF DATABASE_PRINCIPAL_ID(N'MiniStoreAuditWriter') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.database_role_members WHERE role_principal_id = DATABASE_PRINCIPAL_ID(N'MiniStoreAuditWriter'))
                    DROP ROLE [MiniStoreAuditWriter];
                IF DATABASE_PRINCIPAL_ID(N'MiniStoreAuditReader') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.database_role_members WHERE role_principal_id = DATABASE_PRINCIPAL_ID(N'MiniStoreAuditReader'))
                    DROP ROLE [MiniStoreAuditReader];
                IF DATABASE_PRINCIPAL_ID(N'MiniStoreAuditMaintainer') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.database_role_members WHERE role_principal_id = DATABASE_PRINCIPAL_ID(N'MiniStoreAuditMaintainer'))
                    DROP ROLE [MiniStoreAuditMaintainer];
                """);
        }
    }
}
