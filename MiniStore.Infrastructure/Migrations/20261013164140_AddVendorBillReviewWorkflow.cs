using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorBillReviewWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAtUtc",
                table: "VendorBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "VendorBills",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "VendorBills",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc",
                table: "VendorBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUserId",
                table: "VendorBills",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorBillAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorBillId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_VendorBillAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorBillAttachments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorBillAttachments_VendorBills_VendorBillId_TenantId",
                        columns: x => new { x.VendorBillId, x.TenantId },
                        principalTable: "VendorBills",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillAttachments_TenantId",
                table: "VendorBillAttachments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillAttachments_TenantId_VendorBillId_Id",
                table: "VendorBillAttachments",
                columns: new[] { "TenantId", "VendorBillId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorBillAttachments_VendorBillId_TenantId",
                table: "VendorBillAttachments",
                columns: new[] { "VendorBillId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorBillAttachments");

            migrationBuilder.DropColumn(
                name: "ApprovedAtUtc",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "VendorBills");
        }
    }
}
