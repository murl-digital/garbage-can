using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Migrations
{
    public partial class NoMoreStringIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                    "id",
                    "xp_users",
                    "bigint unsigned",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "varchar(18) CHARACTER SET utf8mb4",
                    oldMaxLength: 18)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                    "id",
                    "xp_users",
                    "varchar(18) CHARACTER SET utf8mb4",
                    maxLength: 18,
                    nullable: false,
                    oldClrType: typeof(ulong),
                    oldType: "bigint unsigned")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}