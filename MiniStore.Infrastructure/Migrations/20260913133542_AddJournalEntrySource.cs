using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalEntrySource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "JournalEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "JournalEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_SourceType_SourceReference",
                table: "JournalEntries",
                columns: new[] { "SourceType", "SourceReference" },
                unique: true,
                filter: "[SourceType] IS NOT NULL AND [SourceReference] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_SourceType_SourceReference",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "JournalEntries");
        }
    }
}
