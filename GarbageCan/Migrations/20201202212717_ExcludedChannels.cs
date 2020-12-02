using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class ExcludedChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "xp_excluded_channels",
                columns: table => new
                {
                    channel_id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "xp_excluded_channels");
        }
    }
}
