using Microsoft.EntityFrameworkCore.Migrations;

namespace LearnForeignWords.Migrations
{
    public partial class AddThemeEntity1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThemeId",
                table: "Collection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collection_ThemeId",
                table: "Collection",
                column: "ThemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collection_Themes_ThemeId",
                table: "Collection",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collection_Themes_ThemeId",
                table: "Collection");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropIndex(
                name: "IX_Collection_ThemeId",
                table: "Collection");

            migrationBuilder.DropColumn(
                name: "ThemeId",
                table: "Collection");
        }
    }
}
