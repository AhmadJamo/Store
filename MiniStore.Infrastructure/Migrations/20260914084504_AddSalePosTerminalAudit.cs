using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalePosTerminalAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PosTerminalId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PosTerminalId",
                table: "Sales");
        }
    }
}
