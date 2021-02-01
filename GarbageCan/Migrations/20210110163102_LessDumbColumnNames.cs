using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class LessDumbColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "user_id",
                "xp_user_boosters",
                "userId");

            migrationBuilder.RenameColumn(
                "duration_in_seconds",
                "xp_user_boosters",
                "durationInSeconds");

            migrationBuilder.RenameColumn(
                "duration_in_seconds",
                "xp_queued_boosters",
                "durationInSeconds");

            migrationBuilder.RenameColumn(
                "channel_id",
                "xp_excluded_channels",
                "channelId");

            migrationBuilder.RenameColumn(
                "channel_id",
                "xp_available_slots",
                "channelId");

            migrationBuilder.RenameColumn(
                "expiration_date",
                "xp_active_boosters",
                "expirationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "userId",
                "xp_user_boosters",
                "user_id");

            migrationBuilder.RenameColumn(
                "durationInSeconds",
                "xp_user_boosters",
                "duration_in_seconds");

            migrationBuilder.RenameColumn(
                "durationInSeconds",
                "xp_queued_boosters",
                "duration_in_seconds");

            migrationBuilder.RenameColumn(
                "channelId",
                "xp_excluded_channels",
                "channel_id");

            migrationBuilder.RenameColumn(
                "channelId",
                "xp_available_slots",
                "channel_id");

            migrationBuilder.RenameColumn(
                "expirationDate",
                "xp_active_boosters",
                "expiration_date");
        }
    }
}