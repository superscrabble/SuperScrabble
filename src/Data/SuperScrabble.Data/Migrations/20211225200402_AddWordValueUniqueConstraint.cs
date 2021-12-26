using Microsoft.EntityFrameworkCore.Migrations;

namespace SuperScrabble.Data.Migrations
{
    public partial class AddWordValueUniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_Value",
                table: "Words");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Words",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Value",
                table: "Words",
                column: "Value",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_Value",
                table: "Words");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Words",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.CreateIndex(
                name: "IX_Words_Value",
                table: "Words",
                column: "Value");
        }
    }
}
