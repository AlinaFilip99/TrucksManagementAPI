using Microsoft.EntityFrameworkCore.Migrations;

namespace TrucksManagement.Migrations
{
    public partial class newUserValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriversNumber",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriversNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "AspNetUsers");
        }
    }
}
