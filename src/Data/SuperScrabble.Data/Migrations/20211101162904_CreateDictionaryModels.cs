using Microsoft.EntityFrameworkCore.Migrations;

namespace SuperScrabble.Data.Migrations
{
    public partial class CreateDictionaryModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeaningId = table.Column<int>(type: "int", nullable: false),
                    MainFormId = table.Column<int>(type: "int", nullable: true),
                    LexicalCategoryId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPlural = table.Column<bool>(type: "bit", nullable: true),
                    GrammaticalGenderId = table.Column<int>(type: "int", nullable: true),
                    ArticleType = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    IsImperative = table.Column<bool>(type: "bit", nullable: true),
                    TenseId = table.Column<int>(type: "int", nullable: true),
                    ViewPointId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Words_CommunionTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CommunionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Words_GrammaticalGenders_GrammaticalGenderId",
                        column: x => x.GrammaticalGenderId,
                        principalTable: "GrammaticalGenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Words_LexicalCategories_LexicalCategoryId",
                        column: x => x.LexicalCategoryId,
                        principalTable: "LexicalCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Words_Meanings_MeaningId",
                        column: x => x.MeaningId,
                        principalTable: "Meanings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Words_Tenses_TenseId",
                        column: x => x.TenseId,
                        principalTable: "Tenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Words_ViewPoints_ViewPointId",
                        column: x => x.ViewPointId,
                        principalTable: "ViewPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Words_Words_MainFormId",
                        column: x => x.MainFormId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PronounTypes");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "CommunionTypes");

            migrationBuilder.DropTable(
                name: "GrammaticalGenders");

            migrationBuilder.DropTable(
                name: "LexicalCategories");

            migrationBuilder.DropTable(
                name: "Meanings");

            migrationBuilder.DropTable(
                name: "Tenses");

            migrationBuilder.DropTable(
                name: "ViewPoints");
        }
    }
}
