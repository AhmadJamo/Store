using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesInvoicing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Sales",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Sales",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "Legacy");

            migrationBuilder.CreateTable(
                name: "InvoiceSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WholesalePrefix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PosPrefix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumberLength = table.Column<int>(type: "int", nullable: false),
                    NextWholesaleNumber = table.Column<int>(type: "int", nullable: false),
                    NextPosNumber = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Sales");
        }
    }
}
