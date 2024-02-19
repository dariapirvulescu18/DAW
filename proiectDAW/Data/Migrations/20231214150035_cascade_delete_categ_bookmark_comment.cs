using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proiectDAW.Data.Migrations
{
    public partial class cascade_delete_categ_bookmark_comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Bookmarks_BookmarkId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Bookmarks_BookmarkId",
                table: "Comments",
                column: "BookmarkId",
                principalTable: "Bookmarks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Bookmarks_BookmarkId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Bookmarks_BookmarkId",
                table: "Comments",
                column: "BookmarkId",
                principalTable: "Bookmarks",
                principalColumn: "Id");
        }
    }
}
