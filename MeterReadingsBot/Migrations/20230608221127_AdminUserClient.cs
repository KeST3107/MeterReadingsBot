using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterReadingsBot.Migrations
{
    public partial class AdminUserClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminUserState",
                table: "UserClients",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUserState",
                table: "UserClients");
        }
    }
}
