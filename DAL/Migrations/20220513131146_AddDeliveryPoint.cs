using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace DAL.Migrations
{
    public partial class AddDeliveryPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "DeliveryLocation",
                table: "Order",
                type: "geography",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryLocation",
                table: "Order");
        }
    }
}
