using Microsoft.EntityFrameworkCore.Migrations;

namespace Brandoman.Data.Migrations
{
    public partial class UserLang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Lang",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lang",
                table: "AspNetUsers");
        }
    }
}
