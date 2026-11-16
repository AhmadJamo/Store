using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectErp007PosSessionExclusivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PosCashSessions_TenantId_PosTerminalId_Status",
                table: "PosCashSessions");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_TenantId_PosTerminalId",
                table: "PosCashSessions",
                columns: new[] { "TenantId", "PosTerminalId" },
                unique: true,
                filter: "[Status] IN (1, 2, 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PosCashSessions_TenantId_PosTerminalId",
                table: "PosCashSessions");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_TenantId_PosTerminalId_Status",
                table: "PosCashSessions",
                columns: new[] { "TenantId", "PosTerminalId", "Status" },
                unique: true,
                filter: "[Status] IN (1, 2, 3)");
        }
    }
}
