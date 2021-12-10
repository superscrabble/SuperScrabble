using Microsoft.EntityFrameworkCore.Migrations;

namespace SuperScrabble.Data.Migrations
{
    public partial class AddWords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_CommunionTypes_TypeId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_GrammaticalGenders_GrammaticalGenderId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_LexicalCategories_LexicalCategoryId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Meanings_MeaningId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Tenses_TenseId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_ViewPoints_ViewPointId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Words_MainFormId",
                table: "Words");

            migrationBuilder.DropTable(
                name: "CommunionTypes");

            migrationBuilder.DropTable(
                name: "GrammaticalGenders");

            migrationBuilder.DropTable(
                name: "LexicalCategories");

            migrationBuilder.DropTable(
                name: "Meanings");

            migrationBuilder.DropTable(
                name: "PronounTypes");

            migrationBuilder.DropTable(
                name: "Tenses");

            migrationBuilder.DropTable(
                name: "ViewPoints");

            migrationBuilder.DropIndex(
                name: "IX_Words_GrammaticalGenderId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_LexicalCategoryId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_MainFormId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_MeaningId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_TenseId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_TypeId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_ViewPointId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ArticleType",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "GrammaticalGenderId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "IsImperative",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "IsPlural",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "LexicalCategoryId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "MainFormId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "MeaningId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "TenseId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ViewPointId",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Words",
                newName: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Words",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "ArticleType",
                table: "Words",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Words",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GrammaticalGenderId",
                table: "Words",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImperative",
                table: "Words",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlural",
                table: "Words",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LexicalCategoryId",
                table: "Words",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MainFormId",
                table: "Words",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MeaningId",
                table: "Words",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenseId",
                table: "Words",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Words",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewPointId",
                table: "Words",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommunionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrammaticalGenders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammaticalGenders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LexicalCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LexicalCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meanings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meanings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PronounTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PronounTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewPoints", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_GrammaticalGenderId",
                table: "Words",
                column: "GrammaticalGenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_LexicalCategoryId",
                table: "Words",
                column: "LexicalCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_MainFormId",
                table: "Words",
                column: "MainFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_MeaningId",
                table: "Words",
                column: "MeaningId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_TenseId",
                table: "Words",
                column: "TenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_TypeId",
                table: "Words",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_ViewPointId",
                table: "Words",
                column: "ViewPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_CommunionTypes_TypeId",
                table: "Words",
                column: "TypeId",
                principalTable: "CommunionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_GrammaticalGenders_GrammaticalGenderId",
                table: "Words",
                column: "GrammaticalGenderId",
                principalTable: "GrammaticalGenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_LexicalCategories_LexicalCategoryId",
                table: "Words",
                column: "LexicalCategoryId",
                principalTable: "LexicalCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Meanings_MeaningId",
                table: "Words",
                column: "MeaningId",
                principalTable: "Meanings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Tenses_TenseId",
                table: "Words",
                column: "TenseId",
                principalTable: "Tenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_ViewPoints_ViewPointId",
                table: "Words",
                column: "ViewPointId",
                principalTable: "ViewPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Words_MainFormId",
                table: "Words",
                column: "MainFormId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
