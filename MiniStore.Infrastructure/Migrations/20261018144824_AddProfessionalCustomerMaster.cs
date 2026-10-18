using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalCustomerMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAtUtc",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalName",
                table: "Customers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Customers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Customers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "ar-JO");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Customers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Asia/Amman");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Customers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql("""
                ;WITH NumberedCustomers AS
                (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [TenantId] ORDER BY [Id]) AS [SequenceNumber]
                    FROM [Customers]
                )
                UPDATE c
                SET [CustomerCode] = 'CUS-' + RIGHT(REPLICATE('0', 6) + CONVERT(varchar(20), n.[SequenceNumber]), 6)
                FROM [Customers] c
                INNER JOIN NumberedCustomers n ON n.[Id] = c.[Id];

                INSERT INTO [DocumentSequences]
                    ([TenantId], [DocumentType], [Prefix], [Suffix], [FormatTemplate], [NumberLength],
                     [NextNumber], [ResetStartNumber], [ResetPeriod], [CurrentPeriodKey])
                SELECT t.[Id], 22, 'CUS-', '', '{PREFIX}{NUMBER}{SUFFIX}', 6,
                       COALESCE(COUNT(c.[Id]), 0) + 1, 1, 0, NULL
                FROM [Tenants] t
                LEFT JOIN [Customers] c ON c.[TenantId] = t.[Id]
                WHERE NOT EXISTS
                    (SELECT 1 FROM [DocumentSequences] d WHERE d.[TenantId] = t.[Id] AND d.[DocumentType] = 22)
                GROUP BY t.[Id];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive_Name",
                table: "Customers",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_CustomerCode",
                table: "Customers",
                columns: new[] { "TenantId", "CustomerCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [DocumentSequences] WHERE [DocumentType] = 22;");
            migrationBuilder.DropIndex(
                name: "IX_Customers_IsActive_Name",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_CustomerCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ArchivedAtUtc",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LegalName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Customers");
        }
    }
}
