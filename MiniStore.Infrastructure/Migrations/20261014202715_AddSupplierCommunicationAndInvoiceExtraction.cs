using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierCommunicationAndInvoiceExtraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_VendorBillAttachments_Id_TenantId",
                table: "VendorBillAttachments",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderSupplierConfirmations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    PreviousExpectedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ConfirmedExpectedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ConfirmationReference = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderSupplierConfirmations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderSupplierConfirmations_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderSupplierConfirmations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierCommunications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    DocumentNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCommunications", x => x.Id);
                    table.UniqueConstraint("AK_SupplierCommunications_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SupplierCommunications_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCommunications_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorBillExtractionSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorBillId = table.Column<int>(type: "int", nullable: false),
                    VendorBillAttachmentId = table.Column<int>(type: "int", nullable: false),
                    ExtractionMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RawExtractedText = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: false),
                    SuggestedInvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SuggestedBillDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SuggestedTotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SuggestedCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ConfidencePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBillExtractionSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorBillExtractionSuggestions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillExtractionSuggestions_VendorBillAttachments_VendorBillAttachmentId_TenantId",
                        columns: x => new { x.VendorBillAttachmentId, x.TenantId },
                        principalTable: "VendorBillAttachments",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillExtractionSuggestions_VendorBills_VendorBillId_TenantId",
                        columns: x => new { x.VendorBillId, x.TenantId },
                        principalTable: "VendorBills",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierCommunicationActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierCommunicationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EvidenceReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCommunicationActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierCommunicationActivities_SupplierCommunications_SupplierCommunicationId_TenantId",
                        columns: x => new { x.SupplierCommunicationId, x.TenantId },
                        principalTable: "SupplierCommunications",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierCommunicationActivities_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierCommunicationAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierCommunicationId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    UploadedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCommunicationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierCommunicationAttachments_SupplierCommunications_SupplierCommunicationId_TenantId",
                        columns: x => new { x.SupplierCommunicationId, x.TenantId },
                        principalTable: "SupplierCommunications",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierCommunicationAttachments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderSupplierConfirmations_PurchaseOrderId_RecordedAtUtc",
                table: "PurchaseOrderSupplierConfirmations",
                columns: new[] { "PurchaseOrderId", "RecordedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderSupplierConfirmations_PurchaseOrderId_TenantId",
                table: "PurchaseOrderSupplierConfirmations",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderSupplierConfirmations_TenantId",
                table: "PurchaseOrderSupplierConfirmations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunicationActivities_SupplierCommunicationId_OccurredAtUtc",
                table: "SupplierCommunicationActivities",
                columns: new[] { "SupplierCommunicationId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunicationActivities_SupplierCommunicationId_TenantId",
                table: "SupplierCommunicationActivities",
                columns: new[] { "SupplierCommunicationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunicationActivities_TenantId",
                table: "SupplierCommunicationActivities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunicationAttachments_SupplierCommunicationId_TenantId",
                table: "SupplierCommunicationAttachments",
                columns: new[] { "SupplierCommunicationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunicationAttachments_TenantId",
                table: "SupplierCommunicationAttachments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunicationAttachments_TenantId_SupplierCommunicationId_Id",
                table: "SupplierCommunicationAttachments",
                columns: new[] { "TenantId", "SupplierCommunicationId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunications_DocumentType_DocumentId_CreatedAtUtc",
                table: "SupplierCommunications",
                columns: new[] { "DocumentType", "DocumentId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunications_SupplierId_TenantId",
                table: "SupplierCommunications",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommunications_TenantId",
                table: "SupplierCommunications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillExtractionSuggestions_TenantId",
                table: "VendorBillExtractionSuggestions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillExtractionSuggestions_VendorBillAttachmentId_Status",
                table: "VendorBillExtractionSuggestions",
                columns: new[] { "VendorBillAttachmentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillExtractionSuggestions_VendorBillAttachmentId_TenantId",
                table: "VendorBillExtractionSuggestions",
                columns: new[] { "VendorBillAttachmentId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillExtractionSuggestions_VendorBillId_TenantId",
                table: "VendorBillExtractionSuggestions",
                columns: new[] { "VendorBillId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderSupplierConfirmations");

            migrationBuilder.DropTable(
                name: "SupplierCommunicationActivities");

            migrationBuilder.DropTable(
                name: "SupplierCommunicationAttachments");

            migrationBuilder.DropTable(
                name: "VendorBillExtractionSuggestions");

            migrationBuilder.DropTable(
                name: "SupplierCommunications");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_VendorBillAttachments_Id_TenantId",
                table: "VendorBillAttachments");
        }
    }
}
