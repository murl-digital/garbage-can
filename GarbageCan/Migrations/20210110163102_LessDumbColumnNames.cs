using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class LessDumbColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "xp_user_boosters",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "duration_in_seconds",
                table: "xp_user_boosters",
                newName: "durationInSeconds");

            migrationBuilder.RenameColumn(
                name: "duration_in_seconds",
                table: "xp_queued_boosters",
                newName: "durationInSeconds");

            migrationBuilder.RenameColumn(
                name: "channel_id",
                table: "xp_excluded_channels",
                newName: "channelId");

            migrationBuilder.RenameColumn(
                name: "channel_id",
                table: "xp_available_slots",
                newName: "channelId");

            migrationBuilder.RenameColumn(
                name: "expiration_date",
                table: "xp_active_boosters",
                newName: "expirationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "xp_user_boosters",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "durationInSeconds",
                table: "xp_user_boosters",
                newName: "duration_in_seconds");

            migrationBuilder.RenameColumn(
                name: "durationInSeconds",
                table: "xp_queued_boosters",
                newName: "duration_in_seconds");

            migrationBuilder.RenameColumn(
                name: "channelId",
                table: "xp_excluded_channels",
                newName: "channel_id");

            migrationBuilder.RenameColumn(
                name: "channelId",
                table: "xp_available_slots",
                newName: "channel_id");

            migrationBuilder.RenameColumn(
                name: "expirationDate",
                table: "xp_active_boosters",
                newName: "expiration_date");
        }
    }
}
