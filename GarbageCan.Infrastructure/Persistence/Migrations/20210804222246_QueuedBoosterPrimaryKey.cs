using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    public partial class QueuedBoosterPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "xpQueuedBoosters",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_xpQueuedBoosters",
                table: "xpQueuedBoosters",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_xpQueuedBoosters",
                table: "xpQueuedBoosters");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "xpQueuedBoosters");
        }
    }
}
