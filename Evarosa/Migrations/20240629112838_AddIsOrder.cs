using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOrder",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOrder",
                table: "Products");
        }
    }
}
