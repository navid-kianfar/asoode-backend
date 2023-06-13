using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class shift_working_days : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Friday",
                table: "shifts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Monday",
                table: "shifts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Saturday",
                table: "shifts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sunday",
                table: "shifts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Thursday",
                table: "shifts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Tuesday",
                table: "shifts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Wednesday",
                table: "shifts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Friday",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "Monday",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "Saturday",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "Sunday",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "Thursday",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "Tuesday",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "Wednesday",
                table: "shifts");
        }
    }
}
