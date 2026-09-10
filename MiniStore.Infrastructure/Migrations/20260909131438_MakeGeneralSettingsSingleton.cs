using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeGeneralSettingsSingleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SingletonKey",
                table: "GeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_SingletonKey",
                table: "GeneralSettings",
                column: "SingletonKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeneralSettings_SingletonKey",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "SingletonKey",
                table: "GeneralSettings");
        }
    }
}
