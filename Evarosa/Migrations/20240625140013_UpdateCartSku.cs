using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartSku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "SkuId",
                table: "ShoppingCartItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SkuId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_SkuId",
                table: "ShoppingCartItems",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_SkuId",
                table: "OrderDetails",
                column: "SkuId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Sku_SkuId",
                table: "OrderDetails",
                column: "SkuId",
                principalTable: "Sku",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Sku_SkuId",
                table: "ShoppingCartItems",
                column: "SkuId",
                principalTable: "Sku",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Sku_SkuId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Sku_SkuId",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_SkuId",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_SkuId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SkuId",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "SkuId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
