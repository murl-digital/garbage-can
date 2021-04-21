using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class AvailableSlotsLongId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                "channel_id",
                "xp_available_slots",
                "bigint unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(string),
                oldType: "varchar(18) CHARACTER SET utf8mb4",
                oldMaxLength: 18,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "channel_id",
                "xp_available_slots",
                "varchar(18) CHARACTER SET utf8mb4",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");
        }
    }
}