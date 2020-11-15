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
                name: "xp_available_slots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    channel_id = table.Column<string>(type: "varchar(18) CHARACTER SET utf8mb4", maxLength: 18, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xp_available_slots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "xp_queued_boosters",
                columns: table => new
                {
                    position = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    multiplier = table.Column<float>(type: "float", nullable: false),
                    duration_in_seconds = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xp_queued_boosters", x => x.position);
                });

            migrationBuilder.CreateTable(
                name: "xp_user_boosters",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    user_id = table.Column<string>(type: "varchar(18) CHARACTER SET utf8mb4", maxLength: 18, nullable: true),
                    multiplier = table.Column<float>(type: "float", nullable: false),
                    duration_in_seconds = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xp_user_boosters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "xp_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(18) CHARACTER SET utf8mb4", maxLength: 18, nullable: false),
                    lvl = table.Column<int>(type: "int", nullable: false),
                    xp = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xp_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "xp_active_boosters",
                columns: table => new
                {
                    slot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    slotid = table.Column<int>(type: "int", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    multipler = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xp_active_boosters", x => x.slot_id);
                    table.ForeignKey(
                        name: "FK_xp_active_boosters_xp_available_slots_slotid",
                        column: x => x.slotid,
                        principalTable: "xp_available_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_xp_active_boosters_slotid",
                table: "xp_active_boosters",
                column: "slotid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "xp_active_boosters");

            migrationBuilder.DropTable(
                name: "xp_queued_boosters");

            migrationBuilder.DropTable(
                name: "xp_user_boosters");

            migrationBuilder.DropTable(
                name: "xp_users");

            migrationBuilder.DropTable(
                name: "xp_available_slots");
        }
    }
}
