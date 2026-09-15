using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnifyDocumentNumbering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentSequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FormatTemplate = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NumberLength = table.Column<int>(type: "int", nullable: false),
                    NextNumber = table.Column<long>(type: "bigint", nullable: false),
                    ResetStartNumber = table.Column<long>(type: "bigint", nullable: false),
                    ResetPeriod = table.Column<int>(type: "int", nullable: false),
                    CurrentPeriodKey = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSequences_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSequences_TenantId",
                table: "DocumentSequences",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSequences_TenantId_DocumentType",
                table: "DocumentSequences",
                columns: new[] { "TenantId", "DocumentType" },
                unique: true);

            migrationBuilder.Sql(
                """
                ;WITH RankedInvoiceSettings AS
                (
                    SELECT *, ROW_NUMBER() OVER (PARTITION BY TenantId ORDER BY Id) AS rn
                    FROM InvoiceSettings
                )
                INSERT INTO DocumentSequences
                    (DocumentType, Prefix, Suffix, FormatTemplate, NumberLength, NextNumber,
                     ResetStartNumber, ResetPeriod, CurrentPeriodKey, TenantId)
                SELECT 1, LEFT(WholesalePrefix, 30), N'', N'{PREFIX}{NUMBER}{SUFFIX}',
                       CASE WHEN NumberLength BETWEEN 1 AND 18 THEN NumberLength ELSE 6 END,
                       CASE WHEN NextWholesaleNumber > 0 THEN NextWholesaleNumber ELSE 1 END,
                       1, 0, NULL, TenantId
                FROM RankedInvoiceSettings
                WHERE rn = 1;

                ;WITH RankedInvoiceSettings AS
                (
                    SELECT *, ROW_NUMBER() OVER (PARTITION BY TenantId ORDER BY Id) AS rn
                    FROM InvoiceSettings
                )
                INSERT INTO DocumentSequences
                    (DocumentType, Prefix, Suffix, FormatTemplate, NumberLength, NextNumber,
                     ResetStartNumber, ResetPeriod, CurrentPeriodKey, TenantId)
                SELECT 2, LEFT(PosPrefix, 30), N'', N'{PREFIX}{NUMBER}{SUFFIX}',
                       CASE WHEN NumberLength BETWEEN 1 AND 18 THEN NumberLength ELSE 6 END,
                       CASE WHEN NextPosNumber > 0 THEN NextPosNumber ELSE 1 END,
                       1, 0, NULL, TenantId
                FROM RankedInvoiceSettings
                WHERE rn = 1;

                ;WITH RankedTransferSettings AS
                (
                    SELECT *, ROW_NUMBER() OVER (PARTITION BY TenantId ORDER BY Id) AS rn
                    FROM DocumentNumberSettings
                )
                INSERT INTO DocumentSequences
                    (DocumentType, Prefix, Suffix, FormatTemplate, NumberLength, NextNumber,
                     ResetStartNumber, ResetPeriod, CurrentPeriodKey, TenantId)
                SELECT 3, LEFT(StockTransferPrefix, 30), N'', N'{PREFIX}{NUMBER}{SUFFIX}',
                       CASE WHEN NumberLength BETWEEN 1 AND 18 THEN NumberLength ELSE 6 END,
                       CASE WHEN NextStockTransferNumber > 0 THEN NextStockTransferNumber ELSE 1 END,
                       1, 0, NULL, TenantId
                FROM RankedTransferSettings
                WHERE rn = 1;

                INSERT INTO DocumentSequences
                    (DocumentType, Prefix, Suffix, FormatTemplate, NumberLength, NextNumber,
                     ResetStartNumber, ResetPeriod, CurrentPeriodKey, TenantId)
                SELECT defaults.DocumentType, defaults.Prefix, N'', N'{PREFIX}{NUMBER}{SUFFIX}',
                       6, 1, 1, 0, NULL, tenants.Id
                FROM Tenants AS tenants
                CROSS JOIN (VALUES
                    (1, N'SAL-'),
                    (2, N'POS-'),
                    (3, N'TRF-'),
                    (4, N'JE-')
                ) AS defaults(DocumentType, Prefix)
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM DocumentSequences AS existing
                    WHERE existing.TenantId = tenants.Id
                      AND existing.DocumentType = defaults.DocumentType
                );
                """);

            migrationBuilder.DropTable(
                name: "DocumentNumberSettings");

            migrationBuilder.DropTable(
                name: "InvoiceSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentNumberSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NextStockTransferNumber = table.Column<int>(type: "int", nullable: false),
                    NumberLength = table.Column<int>(type: "int", nullable: false),
                    StockTransferPrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNumberSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentNumberSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NextPosNumber = table.Column<int>(type: "int", nullable: false),
                    NextWholesaleNumber = table.Column<int>(type: "int", nullable: false),
                    NumberLength = table.Column<int>(type: "int", nullable: false),
                    PosPrefix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    WholesalePrefix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumberSettings_TenantId",
                table: "DocumentNumberSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceSettings_TenantId",
                table: "InvoiceSettings",
                column: "TenantId");

            migrationBuilder.Sql(
                """
                INSERT INTO InvoiceSettings
                    (NextPosNumber, NextWholesaleNumber, NumberLength, PosPrefix, TenantId, WholesalePrefix)
                SELECT
                    CASE WHEN pos.NextNumber > 2147483647 THEN 2147483647 ELSE CAST(pos.NextNumber AS int) END,
                    CASE WHEN wholesale.NextNumber > 2147483647 THEN 2147483647 ELSE CAST(wholesale.NextNumber AS int) END,
                    wholesale.NumberLength,
                    LEFT(pos.Prefix, 30),
                    tenants.Id,
                    LEFT(wholesale.Prefix, 30)
                FROM Tenants AS tenants
                OUTER APPLY
                (
                    SELECT TOP (1) NextNumber, NumberLength, Prefix
                    FROM DocumentSequences
                    WHERE TenantId = tenants.Id AND DocumentType = 1
                ) AS wholesale
                OUTER APPLY
                (
                    SELECT TOP (1) NextNumber, Prefix
                    FROM DocumentSequences
                    WHERE TenantId = tenants.Id AND DocumentType = 2
                ) AS pos
                WHERE wholesale.NextNumber IS NOT NULL AND pos.NextNumber IS NOT NULL;

                INSERT INTO DocumentNumberSettings
                    (NextStockTransferNumber, NumberLength, StockTransferPrefix, TenantId)
                SELECT
                    CASE WHEN transferSequence.NextNumber > 2147483647 THEN 2147483647 ELSE CAST(transferSequence.NextNumber AS int) END,
                    transferSequence.NumberLength,
                    LEFT(transferSequence.Prefix, 20),
                    tenants.Id
                FROM Tenants AS tenants
                CROSS APPLY
                (
                    SELECT TOP (1) NextNumber, NumberLength, Prefix
                    FROM DocumentSequences
                    WHERE TenantId = tenants.Id AND DocumentType = 3
                ) AS transferSequence;
                """);

            migrationBuilder.DropTable(
                name: "DocumentSequences");
        }
    }
}
