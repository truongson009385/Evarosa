using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutVideo",
                table: "ConfigSite");

            migrationBuilder.DropColumn(
                name: "Breadcrumb",
                table: "ConfigSite");

            migrationBuilder.RenameColumn(
                name: "Pinterest",
                table: "ConfigSite",
                newName: "Instagram");

            migrationBuilder.AddColumn<string>(
                name: "FooterInfo",
                table: "ConfigSite",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FooterInfo",
                table: "ConfigSite");

            migrationBuilder.RenameColumn(
                name: "Instagram",
                table: "ConfigSite",
                newName: "Pinterest");

            migrationBuilder.AddColumn<string>(
                name: "AboutVideo",
                table: "ConfigSite",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Breadcrumb",
                table: "ConfigSite",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
