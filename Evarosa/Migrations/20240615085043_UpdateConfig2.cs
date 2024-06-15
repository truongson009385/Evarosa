using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfig",
                table: "ConfigSite");

            migrationBuilder.DropColumn(
                name: "PassWordMail",
                table: "ConfigSite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfig",
                table: "ConfigSite",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassWordMail",
                table: "ConfigSite",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
