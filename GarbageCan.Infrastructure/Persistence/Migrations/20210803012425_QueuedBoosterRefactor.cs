using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class QueuedBoosterRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_xpQueuedBoosters",
                table: "xpQueuedBoosters");

            migrationBuilder.AlterColumn<uint>(
                name: "position",
                table: "xpQueuedBoosters",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<ulong>(
                name: "guildId",
                table: "xpQueuedBoosters",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_xpQueuedBoosters_guildId",
                table: "xpQueuedBoosters",
                column: "guildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_xpQueuedBoosters_guildId",
                table: "xpQueuedBoosters");

            migrationBuilder.DropColumn(
                name: "guildId",
                table: "xpQueuedBoosters");

            migrationBuilder.AlterColumn<int>(
                name: "position",
                table: "xpQueuedBoosters",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpQueuedBoosters",
                table: "xpQueuedBoosters",
                column: "position");
        }
    }
}
