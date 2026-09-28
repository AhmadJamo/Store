using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniStore.Infrastructure.Persistence;

#nullable disable

namespace MiniStore.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260928140113_AddManagedMeasurementUnits")]
public partial class AddManagedMeasurementUnits : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MeasurementUnits",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Dimension = table.Column<int>(type: "int", nullable: false),
                FactorToBaseUnit = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                DecimalPlaces = table.Column<int>(type: "int", nullable: false),
                IsSystem = table.Column<bool>(type: "bit", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                table.UniqueConstraint("AK_MeasurementUnits_Id_TenantId", x => new { x.Id, x.TenantId });
                table.ForeignKey(
                    name: "FK_MeasurementUnits_Tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_MeasurementUnits_TenantId",
            table: "MeasurementUnits",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_MeasurementUnits_TenantId_Code",
            table: "MeasurementUnits",
            columns: new[] { "TenantId", "Code" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "MeasurementUnits");
}
