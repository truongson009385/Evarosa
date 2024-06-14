using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductShow2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Display",
                table: "ProductCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Display",
                table: "ProductCategories");
        }
    }
}
