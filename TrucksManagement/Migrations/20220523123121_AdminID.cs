using Microsoft.EntityFrameworkCore.Migrations;

namespace TrucksManagement.Migrations
{
    public partial class AdminID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminID",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminID",
                table: "AspNetUsers");
        }
    }
}
