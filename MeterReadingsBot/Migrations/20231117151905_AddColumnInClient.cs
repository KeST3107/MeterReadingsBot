using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterReadingsBot.Migrations
{
    public partial class AddColumnInClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadingsTime",
                table: "TempClients",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadingsTime",
                table: "TempClients");
        }
    }
}
