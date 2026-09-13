using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTransferLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinationLocationId",
                table: "StockTransferItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceLocationId",
                table: "StockTransferItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_DestinationLocationId",
                table: "StockTransferItems",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_SourceLocationId",
                table: "StockTransferItems",
                column: "SourceLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StorageLocations_DestinationLocationId",
                table: "StockTransferItems",
                column: "DestinationLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StorageLocations_SourceLocationId",
                table: "StockTransferItems",
                column: "SourceLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StorageLocations_DestinationLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StorageLocations_SourceLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_DestinationLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_SourceLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "DestinationLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "SourceLocationId",
                table: "StockTransferItems");
        }
    }
}
