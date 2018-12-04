using Microsoft.EntityFrameworkCore.Migrations;

namespace TheRightDecision.Migrations
{
    public partial class UpdateCreterion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Units",
                table: "Criteria",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Units",
                table: "Criteria",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
