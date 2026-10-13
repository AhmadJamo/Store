using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseDocumentReversals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "LandedCosts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "PurchaseDocumentReversals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReversalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    OriginalDocumentId = table.Column<int>(type: "int", nullable: false),
                    OriginalDocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReversalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReversalJournalEntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDocumentReversals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDocumentReversals_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocumentReversals_TenantId",
                table: "PurchaseDocumentReversals",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocumentReversals_TenantId_DocumentType_OriginalDocumentId",
                table: "PurchaseDocumentReversals",
                columns: new[] { "TenantId", "DocumentType", "OriginalDocumentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocumentReversals_TenantId_IdempotencyKey",
                table: "PurchaseDocumentReversals",
                columns: new[] { "TenantId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocumentReversals_TenantId_ReversalNumber",
                table: "PurchaseDocumentReversals",
                columns: new[] { "TenantId", "ReversalNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseDocumentReversals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LandedCosts");
        }
    }
}
