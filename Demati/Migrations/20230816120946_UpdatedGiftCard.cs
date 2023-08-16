using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demati.Migrations
{
    public partial class UpdatedGiftCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Giftcards",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Giftcards");
        }
    }
}
