using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditEventFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaVersion = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActorType = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SourceChannel = table.Column<int>(type: "int", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CausationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OperationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReasonRedacted = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangesRedactedJson = table.Column<string>(type: "nvarchar(max)", maxLength: 16000, nullable: true),
                    MetadataRedactedJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    RequestPathTemplate = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SourceIpMasked = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IntegrityKeyVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IntegrityHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                    table.CheckConstraint("CK_AuditEvents_Classification", "[SchemaVersion] = 1 AND [Category] BETWEEN 1 AND 6 AND [Severity] BETWEEN 1 AND 3 AND [Outcome] BETWEEN 1 AND 4 AND [ActorType] BETWEEN 1 AND 4 AND [SourceChannel] BETWEEN 1 AND 6");
                    table.CheckConstraint("CK_AuditEvents_Json", "([ChangesRedactedJson] IS NULL OR ISJSON([ChangesRedactedJson]) = 1) AND ([MetadataRedactedJson] IS NULL OR ISJSON([MetadataRedactedJson]) = 1)");
                    table.CheckConstraint("CK_AuditEvents_ScopeTenant", "([Scope] = 1 AND [TenantId] IS NOT NULL AND [TenantId] > 0) OR ([Scope] IN (2, 3) AND [TenantId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_AuditEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_CorrelationId",
                table: "AuditEvents",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_EventId",
                table: "AuditEvents",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_ResourceType_ResourceId_OccurredAtUtc",
                table: "AuditEvents",
                columns: new[] { "ResourceType", "ResourceId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_Scope_TenantId_OperationId",
                table: "AuditEvents",
                columns: new[] { "Scope", "TenantId", "OperationId" },
                unique: true,
                filter: "[OperationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TenantId_OccurredAtUtc_Id",
                table: "AuditEvents",
                columns: new[] { "TenantId", "OccurredAtUtc", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEvents");
        }
    }
}
