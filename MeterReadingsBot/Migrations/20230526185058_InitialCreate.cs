using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterReadingsBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    ColdWaterBathroom = table.Column<int>(type: "integer", nullable: false),
                    HotWaterBathroom = table.Column<int>(type: "integer", nullable: false),
                    ColdWaterKitchen = table.Column<int>(type: "integer", nullable: true),
                    HotWaterKitchen = table.Column<int>(type: "integer", nullable: true),
                    PersonalNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    TimeLastMessage = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DISCRIMINATOR = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: true),
                    TempClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    PersonalNumbers = table.Column<List<string>>(type: "text[]", nullable: true),
                    WaterReadingsState = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClients_TempClients_TempClientId",
                        column: x => x.TempClientId,
                        principalTable: "TempClients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClients_TempClientId",
                table: "UserClients",
                column: "TempClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClients");

            migrationBuilder.DropTable(
                name: "TempClients");
        }
    }
}
