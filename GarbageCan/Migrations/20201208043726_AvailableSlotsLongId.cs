using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class AvailableSlotsLongId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "channel_id",
                table: "xp_available_slots",
                type: "bigint unsigned",
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
                name: "channel_id",
                table: "xp_available_slots",
                type: "varchar(18) CHARACTER SET utf8mb4",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");
        }
    }
}
