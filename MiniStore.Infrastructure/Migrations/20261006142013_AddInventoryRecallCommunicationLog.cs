using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryRecallCommunicationLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_InventoryRecalls_Id_TenantId",
                table: "InventoryRecalls",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "InventoryRecallCommunications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryRecallId = table.Column<long>(type: "bigint", nullable: false),
                    PartyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChannelAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRecallCommunications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRecallCommunications_InventoryRecalls_InventoryRecallId_TenantId",
                        columns: x => new { x.InventoryRecallId, x.TenantId },
                        principalTable: "InventoryRecalls",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryRecallCommunications_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecallCommunications_InventoryRecallId_CreatedAt",
                table: "InventoryRecallCommunications",
                columns: new[] { "InventoryRecallId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecallCommunications_InventoryRecallId_TenantId",
                table: "InventoryRecallCommunications",
                columns: new[] { "InventoryRecallId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecallCommunications_TenantId",
                table: "InventoryRecallCommunications",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryRecallCommunications");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_InventoryRecalls_Id_TenantId",
                table: "InventoryRecalls");
        }
    }
}
