using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class ActiveBoosterRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "multipler",
                table: "xpActiveBoosters",
                newName: "multiplier");

            migrationBuilder.AddColumn<ulong>(
                name: "guildId",
                table: "xpActiveBoosters",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "guildId",
                table: "xpActiveBoosters");

            migrationBuilder.RenameColumn(
                name: "multiplier",
                table: "xpActiveBoosters",
                newName: "multipler");
        }
    }
}
