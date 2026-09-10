using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnforceGeneralSettingsSingleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [dbo].[GeneralSettings] WHERE [SingletonKey] <> 1;");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GeneralSettings_SingletonKey",
                table: "GeneralSettings",
                sql: "[SingletonKey] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GeneralSettings_SingletonKey",
                table: "GeneralSettings");
        }
    }
}