using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierPortalAndIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierExternalAuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    ChannelIdentityId = table.Column<int>(type: "int", nullable: true),
                    Actor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RemoteAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierExternalAuditEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierExternalAuditEvents_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierExternalAuditEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierExternalSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    ChannelIdentityId = table.Column<int>(type: "int", nullable: false),
                    SubmissionType = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", maxLength: 2000000, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ResultType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResultId = table.Column<int>(type: "int", nullable: true),
                    CreatedByActor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierExternalSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierExternalSubmissions_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierExternalSubmissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierIntegrationCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    KeyPrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KeyHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProtectedCallbackSecret = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUsedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierIntegrationCredentials", x => x.Id);
                    table.UniqueConstraint("AK_SupplierIntegrationCredentials_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SupplierIntegrationCredentials_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierIntegrationCredentials_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPortalAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    DocumentNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    LastAccessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPortalAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPortalAccesses_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPortalAccesses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierIntegrationOutboxMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierIntegrationCredentialId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AggregateType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AggregateId = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProtectedAuthorizationSecret = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", maxLength: 2000000, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierIntegrationOutboxMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierIntegrationOutboxMessages_SupplierIntegrationCredentials_SupplierIntegrationCredentialId_TenantId",
                        columns: x => new { x.SupplierIntegrationCredentialId, x.TenantId },
                        principalTable: "SupplierIntegrationCredentials",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierIntegrationOutboxMessages_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalAuditEvents_CreatedAtUtc_Channel",
                table: "SupplierExternalAuditEvents",
                columns: new[] { "CreatedAtUtc", "Channel" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalAuditEvents_SupplierId_TenantId",
                table: "SupplierExternalAuditEvents",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalAuditEvents_TenantId",
                table: "SupplierExternalAuditEvents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalSubmissions_DocumentType_DocumentId_CreatedAtUtc",
                table: "SupplierExternalSubmissions",
                columns: new[] { "DocumentType", "DocumentId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalSubmissions_SupplierId_TenantId",
                table: "SupplierExternalSubmissions",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalSubmissions_TenantId",
                table: "SupplierExternalSubmissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierExternalSubmissions_TenantId_Channel_ChannelIdentityId_IdempotencyKey",
                table: "SupplierExternalSubmissions",
                columns: new[] { "TenantId", "Channel", "ChannelIdentityId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationCredentials_SupplierId_TenantId",
                table: "SupplierIntegrationCredentials",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationCredentials_TenantId",
                table: "SupplierIntegrationCredentials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationCredentials_TenantId_KeyHash",
                table: "SupplierIntegrationCredentials",
                columns: new[] { "TenantId", "KeyHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SupplierIntegrationCredentials_KeyHash_Global",
                table: "SupplierIntegrationCredentials",
                column: "KeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationOutboxMessages_Status_NextAttemptAtUtc",
                table: "SupplierIntegrationOutboxMessages",
                columns: new[] { "Status", "NextAttemptAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationOutboxMessages_SupplierIntegrationCredentialId_TenantId",
                table: "SupplierIntegrationOutboxMessages",
                columns: new[] { "SupplierIntegrationCredentialId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationOutboxMessages_TenantId",
                table: "SupplierIntegrationOutboxMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierIntegrationOutboxMessages_TenantId_IdempotencyKey",
                table: "SupplierIntegrationOutboxMessages",
                columns: new[] { "TenantId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPortalAccesses_DocumentType_DocumentId_SupplierId",
                table: "SupplierPortalAccesses",
                columns: new[] { "DocumentType", "DocumentId", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPortalAccesses_SupplierId_TenantId",
                table: "SupplierPortalAccesses",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPortalAccesses_TenantId",
                table: "SupplierPortalAccesses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPortalAccesses_TenantId_TokenHash",
                table: "SupplierPortalAccesses",
                columns: new[] { "TenantId", "TokenHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SupplierPortalAccesses_TokenHash_Global",
                table: "SupplierPortalAccesses",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierExternalAuditEvents");

            migrationBuilder.DropTable(
                name: "SupplierExternalSubmissions");

            migrationBuilder.DropTable(
                name: "SupplierIntegrationOutboxMessages");

            migrationBuilder.DropTable(
                name: "SupplierPortalAccesses");

            migrationBuilder.DropTable(
                name: "SupplierIntegrationCredentials");
        }
    }
}
