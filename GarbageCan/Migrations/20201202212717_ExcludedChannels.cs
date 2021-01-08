using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class ExcludedChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "xp_excluded_channels",
                table => new
                {
                    channel_id = table.Column<ulong>("bigint unsigned", nullable: false)
                },
                constraints: table => { });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "xp_excluded_channels");
        }
    }
}