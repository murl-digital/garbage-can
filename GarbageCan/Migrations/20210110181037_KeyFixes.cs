using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class KeyFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_level_roles",
                table: "xp_level_roles",
                column: "id");
            
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "xp_level_roles",
                type: "int",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<ulong>(
                name: "roleId",
                table: "xp_level_roles",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "moderation_warnings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "moderation_active_mutes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "moderation_active_channel_restricts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderation_warnings",
                table: "moderation_warnings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderation_active_mutes",
                table: "moderation_active_mutes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderation_active_channel_restricts",
                table: "moderation_active_channel_restricts",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_level_roles",
                table: "xp_level_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderation_warnings",
                table: "moderation_warnings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderation_active_mutes",
                table: "moderation_active_mutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderation_active_channel_restricts",
                table: "moderation_active_channel_restricts");

            migrationBuilder.DropColumn(
                name: "roleId",
                table: "xp_level_roles");

            migrationBuilder.DropColumn(
                name: "id",
                table: "moderation_warnings");

            migrationBuilder.DropColumn(
                name: "id",
                table: "moderation_active_mutes");

            migrationBuilder.DropColumn(
                name: "id",
                table: "moderation_active_channel_restricts");

            migrationBuilder.AlterColumn<ulong>(
                name: "id",
                table: "xp_level_roles",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
