using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOptionGroup5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptionSkus_Option_OptionId",
                table: "OptionSkus");

            migrationBuilder.DropForeignKey(
                name: "FK_OptionSkus_Sku_SkuId",
                table: "OptionSkus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OptionSkus",
                table: "OptionSkus");

            migrationBuilder.RenameTable(
                name: "OptionSkus",
                newName: "OptionSku");

            migrationBuilder.RenameIndex(
                name: "IX_OptionSkus_OptionId",
                table: "OptionSku",
                newName: "IX_OptionSku_OptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OptionSku",
                table: "OptionSku",
                columns: new[] { "SkuId", "OptionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OptionSku_Option_OptionId",
                table: "OptionSku",
                column: "OptionId",
                principalTable: "Option",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OptionSku_Sku_SkuId",
                table: "OptionSku",
                column: "SkuId",
                principalTable: "Sku",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptionSku_Option_OptionId",
                table: "OptionSku");

            migrationBuilder.DropForeignKey(
                name: "FK_OptionSku_Sku_SkuId",
                table: "OptionSku");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OptionSku",
                table: "OptionSku");

            migrationBuilder.RenameTable(
                name: "OptionSku",
                newName: "OptionSkus");

            migrationBuilder.RenameIndex(
                name: "IX_OptionSku_OptionId",
                table: "OptionSkus",
                newName: "IX_OptionSkus_OptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OptionSkus",
                table: "OptionSkus",
                columns: new[] { "SkuId", "OptionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OptionSkus_Option_OptionId",
                table: "OptionSkus",
                column: "OptionId",
                principalTable: "Option",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OptionSkus_Sku_SkuId",
                table: "OptionSkus",
                column: "SkuId",
                principalTable: "Sku",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
