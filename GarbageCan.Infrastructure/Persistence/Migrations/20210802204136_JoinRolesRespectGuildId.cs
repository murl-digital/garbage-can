using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class JoinRolesRespectGuildId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_joinWatchlist",
                table: "joinWatchlist");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "joinWatchlist",
                newName: "userId");

            migrationBuilder.AlterColumn<ulong>(
                name: "userId",
                table: "joinWatchlist",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<ulong>(
                name: "guildId",
                table: "joinWatchlist",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "guildId",
                table: "joinRoles",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_joinWatchlist_guildId",
                table: "joinWatchlist",
                column: "guildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_joinWatchlist_guildId",
                table: "joinWatchlist");

            migrationBuilder.DropColumn(
                name: "guildId",
                table: "joinWatchlist");

            migrationBuilder.DropColumn(
                name: "guildId",
                table: "joinRoles");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "joinWatchlist",
                newName: "id");

            migrationBuilder.AlterColumn<ulong>(
                name: "id",
                table: "joinWatchlist",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_joinWatchlist",
                table: "joinWatchlist",
                column: "id");
        }
    }
}
