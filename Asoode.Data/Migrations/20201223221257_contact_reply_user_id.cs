﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class contact_reply_user_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "contactreplies",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "contactreplies");
        }
    }
}
