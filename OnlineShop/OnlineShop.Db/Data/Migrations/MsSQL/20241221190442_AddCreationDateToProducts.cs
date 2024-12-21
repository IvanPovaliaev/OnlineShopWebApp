using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Infrastructure.Migrations.MsSQL
{
    /// <inheritdoc />
    public partial class AddCreationDateToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("0025491a-1eeb-4e6d-8dc9-26fc69ecb1b0"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("202d752c-be4e-4f32-a7a0-433bcc1f2bb4"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("3b232651-c925-4eff-a481-e9d09b503377"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("5c2a6e43-0e05-47a2-9c92-eb839f0b3e63"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c643c046-93df-4d0f-ba01-224b689eca0f"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d13b5481-bfe0-4d5c-885b-1d43df8aa6b9"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("db040c0c-f8f4-48fd-8be4-3040caaa722e"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("fa991ad4-510b-47d2-848c-130aadd43838"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("ffc4ec07-f264-4930-8892-e22cad344f51"),
                column: "CreationDate",
                value: new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Products");
        }
    }
}
