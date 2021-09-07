using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class NoMoreTableAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_xpLevelRoles",
                table: "xpLevelRoles");

            migrationBuilder.RenameTable(
                name: "xpLevelRoles",
                newName: "levelRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_levelRoles",
                table: "levelRoles",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_levelRoles",
                table: "levelRoles");

            migrationBuilder.RenameTable(
                name: "levelRoles",
                newName: "xpLevelRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpLevelRoles",
                table: "xpLevelRoles",
                column: "id");
        }
    }
}
