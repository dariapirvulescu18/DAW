using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proiectDAW.Data.Migrations
{
    public partial class PhotoCover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo_Cover",
                table: "Bookmarks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo_Cover",
                table: "Bookmarks");
        }
    }
}
