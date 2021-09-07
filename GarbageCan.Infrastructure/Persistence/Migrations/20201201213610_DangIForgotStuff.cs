using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class DangIForgotStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                    "id",
                    "xp_users",
                    "bigint unsigned",
                    nullable: false,
                    oldClrType: typeof(ulong),
                    oldType: "bigint unsigned")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ulong>(
                "user_id",
                "xp_user_boosters",
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
            migrationBuilder.AlterColumn<ulong>(
                    "id",
                    "xp_users",
                    "bigint unsigned",
                    nullable: false,
                    oldClrType: typeof(ulong),
                    oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                "user_id",
                "xp_user_boosters",
                "varchar(18) CHARACTER SET utf8mb4",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");
        }
    }
}