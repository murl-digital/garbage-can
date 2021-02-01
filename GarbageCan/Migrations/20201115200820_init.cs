using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "xp_available_slots",
                table => new
                {
                    id = table.Column<int>("int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    channel_id = table.Column<string>("varchar(18) CHARACTER SET utf8mb4", maxLength: 18,
                        nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_xp_available_slots", x => x.id); });

            migrationBuilder.CreateTable(
                "xp_queued_boosters",
                table => new
                {
                    position = table.Column<int>("int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    multiplier = table.Column<float>("float", nullable: false),
                    duration_in_seconds = table.Column<long>("bigint", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_xp_queued_boosters", x => x.position); });

            migrationBuilder.CreateTable(
                "xp_user_boosters",
                table => new
                {
                    id = table.Column<string>("varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    user_id = table.Column<string>("varchar(18) CHARACTER SET utf8mb4", maxLength: 18, nullable: true),
                    multiplier = table.Column<float>("float", nullable: false),
                    duration_in_seconds = table.Column<long>("bigint", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_xp_user_boosters", x => x.id); });

            migrationBuilder.CreateTable(
                "xp_users",
                table => new
                {
                    id = table.Column<string>("varchar(18) CHARACTER SET utf8mb4", maxLength: 18, nullable: false),
                    lvl = table.Column<int>("int", nullable: false),
                    xp = table.Column<double>("double", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_xp_users", x => x.id); });

            migrationBuilder.CreateTable(
                "xp_active_boosters",
                table => new
                {
                    slot_id = table.Column<int>("int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    slotid = table.Column<int>("int", nullable: true),
                    expiration_date = table.Column<DateTime>("datetime", nullable: false),
                    multipler = table.Column<float>("float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xp_active_boosters", x => x.slot_id);
                    table.ForeignKey(
                        "FK_xp_active_boosters_xp_available_slots_slotid",
                        x => x.slotid,
                        "xp_available_slots",
                        "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_xp_active_boosters_slotid",
                "xp_active_boosters",
                "slotid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "xp_active_boosters");

            migrationBuilder.DropTable(
                "xp_queued_boosters");

            migrationBuilder.DropTable(
                "xp_user_boosters");

            migrationBuilder.DropTable(
                "xp_users");

            migrationBuilder.DropTable(
                "xp_available_slots");
        }
    }
}