using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class activebooster_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_xp_active_boosters_xp_available_slots_slotid",
                "xp_active_boosters");

            migrationBuilder.DropIndex(
                "IX_xp_active_boosters_slotid",
                "xp_active_boosters");

            migrationBuilder.DropColumn(
                "slotid",
                "xp_active_boosters");

            migrationBuilder.RenameColumn(
                "slot_id",
                "xp_active_boosters",
                "id");

            migrationBuilder.AlterColumn<int>(
                    "id",
                    "xp_active_boosters",
                    "int",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                "FK_xp_active_boosters_xp_available_slots_id",
                "xp_active_boosters",
                "id",
                "xp_available_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_xp_active_boosters_xp_available_slots_id",
                "xp_active_boosters");

            migrationBuilder.RenameColumn(
                "id",
                "xp_active_boosters",
                "slot_id");

            migrationBuilder.AlterColumn<int>(
                    "slot_id",
                    "xp_active_boosters",
                    "int",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                "slotid",
                "xp_active_boosters",
                "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                "IX_xp_active_boosters_slotid",
                "xp_active_boosters",
                "slotid");

            migrationBuilder.AddForeignKey(
                "FK_xp_active_boosters_xp_available_slots_slotid",
                "xp_active_boosters",
                "slotid",
                "xp_available_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}