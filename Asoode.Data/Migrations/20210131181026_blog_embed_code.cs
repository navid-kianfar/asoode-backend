using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class blog_embed_code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmbedCode",
                table: "blogposts",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbedCode",
                table: "blogposts");
        }
    }
}
