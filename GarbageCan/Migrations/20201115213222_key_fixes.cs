using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
	public partial class key_fixes : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
					"position",
					"xp_queued_boosters",
					"int",
					nullable: false,
					oldClrType: typeof(int),
					oldType: "int")
				.OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
					"position",
					"xp_queued_boosters",
					"int",
					nullable: false,
					oldClrType: typeof(int),
					oldType: "int")
				.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
		}
	}
}