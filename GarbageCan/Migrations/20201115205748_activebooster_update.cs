using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class activebooster_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_xp_active_boosters_xp_available_slots_slotid",
                table: "xp_active_boosters");

            migrationBuilder.DropIndex(
                name: "IX_xp_active_boosters_slotid",
                table: "xp_active_boosters");

            migrationBuilder.DropColumn(
                name: "slotid",
                table: "xp_active_boosters");

            migrationBuilder.RenameColumn(
                name: "slot_id",
                table: "xp_active_boosters",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "xp_active_boosters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_xp_active_boosters_xp_available_slots_id",
                table: "xp_active_boosters",
                column: "id",
                principalTable: "xp_available_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_xp_active_boosters_xp_available_slots_id",
                table: "xp_active_boosters");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "xp_active_boosters",
                newName: "slot_id");

            migrationBuilder.AlterColumn<int>(
                name: "slot_id",
                table: "xp_active_boosters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "slotid",
                table: "xp_active_boosters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_xp_active_boosters_slotid",
                table: "xp_active_boosters",
                column: "slotid");

            migrationBuilder.AddForeignKey(
                name: "FK_xp_active_boosters_xp_available_slots_slotid",
                table: "xp_active_boosters",
                column: "slotid",
                principalTable: "xp_available_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
