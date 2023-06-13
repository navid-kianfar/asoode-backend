using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class package_task_sort_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "AttachmentsSort",
                table: "workpackagetasks",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SubTasksSort",
                table: "workpackagetasks",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "AttachmentsSort",
                table: "workpackages",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ListsSort",
                table: "workpackages",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SubTasksSort",
                table: "workpackages",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TasksSort",
                table: "workpackages",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentsSort",
                table: "workpackagetasks");

            migrationBuilder.DropColumn(
                name: "SubTasksSort",
                table: "workpackagetasks");

            migrationBuilder.DropColumn(
                name: "AttachmentsSort",
                table: "workpackages");

            migrationBuilder.DropColumn(
                name: "ListsSort",
                table: "workpackages");

            migrationBuilder.DropColumn(
                name: "SubTasksSort",
                table: "workpackages");

            migrationBuilder.DropColumn(
                name: "TasksSort",
                table: "workpackages");
        }
    }
}
