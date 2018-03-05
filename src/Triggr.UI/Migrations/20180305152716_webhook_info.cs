using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Triggr.UI.Migrations
{
    public partial class webhook_info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Repositories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Repositories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Repositories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WebHook",
                table: "Repositories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WebHookId",
                table: "Repositories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "WebHook",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "WebHookId",
                table: "Repositories");
        }
    }
}
