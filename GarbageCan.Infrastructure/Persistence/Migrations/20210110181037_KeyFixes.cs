using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class KeyFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                "PK_xp_level_roles",
                "xp_level_roles",
                "id");

            migrationBuilder.AlterColumn<int>(
                    "id",
                    "xp_level_roles",
                    "int",
                    nullable: false,
                    oldClrType: typeof(ulong),
                    oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<ulong>(
                "roleId",
                "xp_level_roles",
                "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<int>(
                    "id",
                    "moderation_warnings",
                    "int",
                    nullable: false,
                    defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                    "id",
                    "moderation_active_mutes",
                    "int",
                    nullable: false,
                    defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                    "id",
                    "moderation_active_channel_restricts",
                    "int",
                    nullable: false,
                    defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                "PK_moderation_warnings",
                "moderation_warnings",
                "id");

            migrationBuilder.AddPrimaryKey(
                "PK_moderation_active_mutes",
                "moderation_active_mutes",
                "id");

            migrationBuilder.AddPrimaryKey(
                "PK_moderation_active_channel_restricts",
                "moderation_active_channel_restricts",
                "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                "PK_xp_level_roles",
                "xp_level_roles");

            migrationBuilder.DropPrimaryKey(
                "PK_moderation_warnings",
                "moderation_warnings");

            migrationBuilder.DropPrimaryKey(
                "PK_moderation_active_mutes",
                "moderation_active_mutes");

            migrationBuilder.DropPrimaryKey(
                "PK_moderation_active_channel_restricts",
                "moderation_active_channel_restricts");

            migrationBuilder.DropColumn(
                "roleId",
                "xp_level_roles");

            migrationBuilder.DropColumn(
                "id",
                "moderation_warnings");

            migrationBuilder.DropColumn(
                "id",
                "moderation_active_mutes");

            migrationBuilder.DropColumn(
                "id",
                "moderation_active_channel_restricts");

            migrationBuilder.AlterColumn<ulong>(
                    "id",
                    "xp_level_roles",
                    "bigint unsigned",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}