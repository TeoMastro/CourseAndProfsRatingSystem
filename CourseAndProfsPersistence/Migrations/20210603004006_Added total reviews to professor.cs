using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseAndProfsPersistence.Migrations
{
    public partial class Addedtotalreviewstoprofessor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalReviews",
                table: "Professors",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalReviews",
                table: "Professors");
        }
    }
}
