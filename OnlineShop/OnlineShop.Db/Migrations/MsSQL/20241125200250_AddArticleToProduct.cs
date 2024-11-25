using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Db.Migrations.MsSQL
{
    /// <inheritdoc />
    public partial class AddArticleToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Article",
                table: "Products",
                type: "bigint",
                nullable: false,
                computedColumnSql: "ABS(CAST(CONVERT(BINARY(8), Id) AS BIGINT))",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "Products");
        }
    }
}
