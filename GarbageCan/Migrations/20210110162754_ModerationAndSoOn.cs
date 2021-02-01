using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class ModerationAndSoOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "moderation_active_channel_restricts",
                table => new
                {
                    uId = table.Column<ulong>("bigint unsigned", nullable: false),
                    channelId = table.Column<ulong>("bigint unsigned", nullable: false),
                    expirationDate = table.Column<DateTime>("datetime", nullable: false)
                },
                constraints: table => { });

            migrationBuilder.CreateTable(
                "moderation_active_mutes",
                table => new
                {
                    uId = table.Column<ulong>("bigint unsigned", nullable: false),
                    expirationDate = table.Column<DateTime>("datetime", nullable: false)
                },
                constraints: table => { });

            migrationBuilder.CreateTable(
                "moderation_warnings",
                table => new
                {
                    uId = table.Column<ulong>("bigint unsigned", nullable: false),
                    mId = table.Column<ulong>("bigint unsigned", nullable: false),
                    issuedDate = table.Column<DateTime>("datetime", nullable: false),
                    punishmentLevel = table.Column<int>("int", nullable: false),
                    comments = table.Column<string>("longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table => { });

            migrationBuilder.CreateTable(
                "xp_level_roles",
                table => new
                {
                    lvl = table.Column<int>("int", nullable: false),
                    id = table.Column<ulong>("bigint unsigned", nullable: false),
                    remain = table.Column<bool>("tinyint(1)", nullable: false)
                },
                constraints: table => { });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "moderation_active_channel_restricts");

            migrationBuilder.DropTable(
                "moderation_active_mutes");

            migrationBuilder.DropTable(
                "moderation_warnings");

            migrationBuilder.DropTable(
                "xp_level_roles");
        }
    }
}