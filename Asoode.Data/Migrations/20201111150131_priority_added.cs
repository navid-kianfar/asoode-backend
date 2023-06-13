using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class priority_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "blogposts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "blogposts");
        }
    }
}
