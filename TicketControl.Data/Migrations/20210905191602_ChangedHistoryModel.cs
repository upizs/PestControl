using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketControl.Data.Migrations
{
    public partial class ChangedHistoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Histories");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Histories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "Histories");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Histories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
