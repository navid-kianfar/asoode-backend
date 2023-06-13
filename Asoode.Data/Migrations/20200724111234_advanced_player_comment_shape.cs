using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class advanced_player_comment_shape : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "advancedplayercomments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    StartFrame = table.Column<int>(nullable: false),
                    EndFrame = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: true),
                    Payload = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advancedplayercomments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "advancedplayershapes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    StartFrame = table.Column<int>(nullable: false),
                    EndFrame = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advancedplayershapes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "advancedplayercomments");

            migrationBuilder.DropTable(
                name: "advancedplayershapes");
        }
    }
}
