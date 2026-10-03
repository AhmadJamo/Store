using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferMovementTransitStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_LocationMovementId",
                table: "StockMovements");

            migrationBuilder.AlterColumn<int>(
                name: "ToStorageLocationId",
                table: "StockMovements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "LocationMovementId",
                table: "StockMovements",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostedByUserId",
                table: "StockMovements",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelatedWarehouseId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReversedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReversedByUserId",
                table: "StockMovements",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceDocumentId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDocumentType",
                table: "StockMovements",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceLineId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StageSequence",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE [StockMovements] SET [CreatedAt] = [PostedAt], [PostedByUserId] = [CreatedByUserId] WHERE [CreatedAt] IS NULL;");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_RelatedWarehouseId_TenantId",
                table: "StockMovements",
                columns: new[] { "RelatedWarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_LocationMovementId",
                table: "StockMovements",
                columns: new[] { "TenantId", "LocationMovementId" },
                unique: true,
                filter: "[LocationMovementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_SourceDocumentType_SourceDocumentId_SourceLineId_StageSequence",
                table: "StockMovements",
                columns: new[] { "TenantId", "SourceDocumentType", "SourceDocumentId", "SourceLineId", "StageSequence" },
                unique: true,
                filter: "[SourceDocumentType] IS NOT NULL AND [SourceDocumentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Warehouses_RelatedWarehouseId_TenantId",
                table: "StockMovements",
                columns: new[] { "RelatedWarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Warehouses_RelatedWarehouseId_TenantId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_RelatedWarehouseId_TenantId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_LocationMovementId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_SourceDocumentType_SourceDocumentId_SourceLineId_StageSequence",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "PostedByUserId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "RelatedWarehouseId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReversedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReversedByUserId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceDocumentId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceDocumentType",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceLineId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "StageSequence",
                table: "StockMovements");

            migrationBuilder.AlterColumn<int>(
                name: "ToStorageLocationId",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "LocationMovementId",
                table: "StockMovements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_LocationMovementId",
                table: "StockMovements",
                columns: new[] { "TenantId", "LocationMovementId" },
                unique: true);
        }
    }
}
