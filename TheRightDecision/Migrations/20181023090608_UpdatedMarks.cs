using Microsoft.EntityFrameworkCore.Migrations;

namespace TheRightDecision.Migrations
{
    public partial class UpdatedMarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Number",
                table: "Marks",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Normalized",
                table: "Marks",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Marks",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Normalized",
                table: "Marks",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
