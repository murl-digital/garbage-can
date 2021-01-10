using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class ModerationAndSoOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "moderation_active_channel_restricts",
                columns: table => new
                {
                    uId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    channelId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    expirationDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "moderation_active_mutes",
                columns: table => new
                {
                    uId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    expirationDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "moderation_warnings",
                columns: table => new
                {
                    uId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    mId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    issuedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    punishmentLevel = table.Column<int>(type: "int", nullable: false),
                    comments = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "xp_level_roles",
                columns: table => new
                {
                    lvl = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    remain = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderation_active_channel_restricts");

            migrationBuilder.DropTable(
                name: "moderation_active_mutes");

            migrationBuilder.DropTable(
                name: "moderation_warnings");

            migrationBuilder.DropTable(
                name: "xp_level_roles");
        }
    }
}
