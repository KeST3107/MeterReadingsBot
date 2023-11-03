using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterReadingsBot.Migrations
{
    public partial class UserNameForAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserClients",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserClients");
        }
    }
}
