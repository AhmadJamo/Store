using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAllocationAndPurchaseItemWarehouses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "PurchaseItems",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE pi SET pi.WarehouseId = p.WarehouseId FROM PurchaseItems pi INNER JOIN Purchases p ON p.Id = pi.PurchaseId WHERE pi.WarehouseId IS NULL");

            migrationBuilder.CreateTable(
                name: "ProductLocationStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLocationStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLocationStocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLocationStocks_StorageLocations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLocationStocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_WarehouseId",
                table: "PurchaseItems",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_ProductId_StorageLocationId",
                table: "ProductLocationStocks",
                columns: new[] { "ProductId", "StorageLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_StorageLocationId",
                table: "ProductLocationStocks",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_WarehouseId",
                table: "ProductLocationStocks",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId",
                table: "PurchaseItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId",
                table: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_WarehouseId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "PurchaseItems");
        }
    }
}
