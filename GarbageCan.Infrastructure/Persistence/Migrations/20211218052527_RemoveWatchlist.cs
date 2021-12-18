using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class RemoveWatchlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "joinWatchlist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "joinWatchlist",
                columns: table => new
                {
                    guildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    userId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_joinWatchlist_guildId",
                table: "joinWatchlist",
                column: "guildId");
        }
    }
}
