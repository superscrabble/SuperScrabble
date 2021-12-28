using Microsoft.EntityFrameworkCore.Migrations;

namespace SuperScrabble.Data.Migrations
{
    public partial class AddHasLeftColumnToUserGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasLeft",
                table: "UsersGames",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLeft",
                table: "UsersGames");
        }
    }
}
