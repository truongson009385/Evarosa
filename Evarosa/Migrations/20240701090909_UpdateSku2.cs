using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSku2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "ArticleCategories");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Sku",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Sku");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ArticleCategories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
