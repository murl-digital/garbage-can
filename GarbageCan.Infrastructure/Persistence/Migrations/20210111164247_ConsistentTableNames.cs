using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class ConsistentTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_xp_active_boosters_xp_available_slots_id",
                table: "xp_active_boosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_users",
                table: "xp_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_user_boosters",
                table: "xp_user_boosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_queued_boosters",
                table: "xp_queued_boosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_level_roles",
                table: "xp_level_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_available_slots",
                table: "xp_available_slots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xp_active_boosters",
                table: "xp_active_boosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderation_warnings",
                table: "moderation_warnings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderation_active_mutes",
                table: "moderation_active_mutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderation_active_channel_restricts",
                table: "moderation_active_channel_restricts");

            migrationBuilder.RenameTable(
                name: "xp_users",
                newName: "xpUsers");

            migrationBuilder.RenameTable(
                name: "xp_user_boosters",
                newName: "xpUserBoosters");

            migrationBuilder.RenameTable(
                name: "xp_queued_boosters",
                newName: "xpQueuedBoosters");

            migrationBuilder.RenameTable(
                name: "xp_level_roles",
                newName: "xpLevelRoles");

            migrationBuilder.RenameTable(
                name: "xp_excluded_channels",
                newName: "xpExcludedChannels");

            migrationBuilder.RenameTable(
                name: "xp_available_slots",
                newName: "xpAvailableSlots");

            migrationBuilder.RenameTable(
                name: "xp_active_boosters",
                newName: "xpActiveBoosters");

            migrationBuilder.RenameTable(
                name: "moderation_warnings",
                newName: "moderationActionLogs");

            migrationBuilder.RenameTable(
                name: "moderation_active_mutes",
                newName: "moderationActiveMutes");

            migrationBuilder.RenameTable(
                name: "moderation_active_channel_restricts",
                newName: "moderationActiveChannelRestricts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpUsers",
                table: "xpUsers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpUserBoosters",
                table: "xpUserBoosters",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpQueuedBoosters",
                table: "xpQueuedBoosters",
                column: "position");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpLevelRoles",
                table: "xpLevelRoles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpAvailableSlots",
                table: "xpAvailableSlots",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpActiveBoosters",
                table: "xpActiveBoosters",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderationActionLogs",
                table: "moderationActionLogs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderationActiveMutes",
                table: "moderationActiveMutes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderationActiveChannelRestricts",
                table: "moderationActiveChannelRestricts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_xpActiveBoosters_xpAvailableSlots_id",
                table: "xpActiveBoosters",
                column: "id",
                principalTable: "xpAvailableSlots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_xpActiveBoosters_xpAvailableSlots_id",
                table: "xpActiveBoosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xpUsers",
                table: "xpUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xpUserBoosters",
                table: "xpUserBoosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xpQueuedBoosters",
                table: "xpQueuedBoosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xpLevelRoles",
                table: "xpLevelRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xpAvailableSlots",
                table: "xpAvailableSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_xpActiveBoosters",
                table: "xpActiveBoosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderationActiveMutes",
                table: "moderationActiveMutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderationActiveChannelRestricts",
                table: "moderationActiveChannelRestricts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderationActionLogs",
                table: "moderationActionLogs");

            migrationBuilder.RenameTable(
                name: "xpUsers",
                newName: "xp_users");

            migrationBuilder.RenameTable(
                name: "xpUserBoosters",
                newName: "xp_user_boosters");

            migrationBuilder.RenameTable(
                name: "xpQueuedBoosters",
                newName: "xp_queued_boosters");

            migrationBuilder.RenameTable(
                name: "xpLevelRoles",
                newName: "xp_level_roles");

            migrationBuilder.RenameTable(
                name: "xpExcludedChannels",
                newName: "xp_excluded_channels");

            migrationBuilder.RenameTable(
                name: "xpAvailableSlots",
                newName: "xp_available_slots");

            migrationBuilder.RenameTable(
                name: "xpActiveBoosters",
                newName: "xp_active_boosters");

            migrationBuilder.RenameTable(
                name: "moderationActiveMutes",
                newName: "moderation_active_mutes");

            migrationBuilder.RenameTable(
                name: "moderationActiveChannelRestricts",
                newName: "moderation_active_channel_restricts");

            migrationBuilder.RenameTable(
                name: "moderationActionLogs",
                newName: "moderation_warnings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_users",
                table: "xp_users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_user_boosters",
                table: "xp_user_boosters",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_queued_boosters",
                table: "xp_queued_boosters",
                column: "position");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_level_roles",
                table: "xp_level_roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_available_slots",
                table: "xp_available_slots",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_xp_active_boosters",
                table: "xp_active_boosters",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderation_active_mutes",
                table: "moderation_active_mutes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderation_active_channel_restricts",
                table: "moderation_active_channel_restricts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderation_warnings",
                table: "moderation_warnings",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_xp_active_boosters_xp_available_slots_id",
                table: "xp_active_boosters",
                column: "id",
                principalTable: "xp_available_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
