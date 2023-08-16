using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demati.Migrations
{
    public partial class UpdaterOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserId",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Products_ProductId",
                table: "Wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wishlists",
                table: "Wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GiftCards",
                table: "GiftCards");

            migrationBuilder.RenameTable(
                name: "Wishlists",
                newName: "wishlists");

            migrationBuilder.RenameTable(
                name: "GiftCards",
                newName: "giftcards");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_UserId",
                table: "wishlists",
                newName: "IX_wishlists_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_ProductId",
                table: "wishlists",
                newName: "IX_wishlists_ProductId");

            migrationBuilder.AddColumn<double>(
                name: "Shipping",
                table: "OrderItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_wishlists",
                table: "wishlists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_giftcards",
                table: "giftcards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_wishlists_AspNetUsers_UserId",
                table: "wishlists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_wishlists_Products_ProductId",
                table: "wishlists",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wishlists_AspNetUsers_UserId",
                table: "wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_wishlists_Products_ProductId",
                table: "wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_wishlists",
                table: "wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_giftcards",
                table: "giftcards");

            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "OrderItems");

            migrationBuilder.RenameTable(
                name: "wishlists",
                newName: "Wishlists");

            migrationBuilder.RenameTable(
                name: "giftcards",
                newName: "GiftCards");

            migrationBuilder.RenameIndex(
                name: "IX_wishlists_UserId",
                table: "Wishlists",
                newName: "IX_Wishlists_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_wishlists_ProductId",
                table: "Wishlists",
                newName: "IX_Wishlists_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wishlists",
                table: "Wishlists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GiftCards",
                table: "GiftCards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserId",
                table: "Wishlists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Products_ProductId",
                table: "Wishlists",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
