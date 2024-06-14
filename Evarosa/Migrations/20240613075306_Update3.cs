using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Banners_BannerPromotionId",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_BannerPromotionId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ShortDes",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TextOrder",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Banner",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "BannerPromotionId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "Des",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "TextSize",
                table: "Products",
                newName: "ShortDescription");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "ProductCategories",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Products",
                newName: "TextSize");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ProductCategories",
                newName: "Icon");

            migrationBuilder.AddColumn<string>(
                name: "ShortDes",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextOrder",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Banner",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BannerPromotionId",
                table: "ProductCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Des",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_BannerPromotionId",
                table: "ProductCategories",
                column: "BannerPromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Banners_BannerPromotionId",
                table: "ProductCategories",
                column: "BannerPromotionId",
                principalTable: "Banners",
                principalColumn: "Id");
        }
    }
}
