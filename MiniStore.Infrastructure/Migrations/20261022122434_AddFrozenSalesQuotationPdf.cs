using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFrozenSalesQuotationPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesQuotationDocumentSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesQuotationId = table.Column<int>(type: "int", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogoContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContentHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesQuotationDocumentSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesQuotationDocumentSnapshots_SalesQuotations_SalesQuotationId_TenantId",
                        columns: x => new { x.SalesQuotationId, x.TenantId },
                        principalTable: "SalesQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesQuotationDocumentSnapshots_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationDocumentSnapshots_SalesQuotationId_TenantId",
                table: "SalesQuotationDocumentSnapshots",
                columns: new[] { "SalesQuotationId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationDocumentSnapshots_TenantId",
                table: "SalesQuotationDocumentSnapshots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationDocumentSnapshots_TenantId_SalesQuotationId",
                table: "SalesQuotationDocumentSnapshots",
                columns: new[] { "TenantId", "SalesQuotationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesQuotationDocumentSnapshots");
        }
    }
}
