using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class order_changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalComplexGroup",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "AdditionalProject",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "AdditionalSimpleGroup",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "AdditionalSpace",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "AdditionalUser",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "AdditionalWorkPackage",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "Yearly",
                table: "userplaninfo");

            migrationBuilder.DropColumn(
                name: "AdditionalComplexGroup",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "AdditionalProject",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "AdditionalSimpleGroup",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "AdditionalSpace",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "AdditionalUser",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "AdditionalWorkPackage",
                table: "orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdditionalComplexGroup",
                table: "userplaninfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalProject",
                table: "userplaninfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalSimpleGroup",
                table: "userplaninfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalSpace",
                table: "userplaninfo",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalUser",
                table: "userplaninfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalWorkPackage",
                table: "userplaninfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Yearly",
                table: "userplaninfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalComplexGroup",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalProject",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalSimpleGroup",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "AdditionalSpace",
                table: "orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalUser",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalWorkPackage",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
