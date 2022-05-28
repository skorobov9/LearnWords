using Microsoft.EntityFrameworkCore.Migrations;

namespace LearnForeignWords.Migrations
{
    public partial class AddImageToThemeEntity1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Themes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Themes");
        }
    }
}
