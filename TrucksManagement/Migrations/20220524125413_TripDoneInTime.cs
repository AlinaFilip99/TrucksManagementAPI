using Microsoft.EntityFrameworkCore.Migrations;

namespace TrucksManagement.Migrations
{
    public partial class TripDoneInTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDoneInTime",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDoneInTime",
                table: "Trips");
        }
    }
}
