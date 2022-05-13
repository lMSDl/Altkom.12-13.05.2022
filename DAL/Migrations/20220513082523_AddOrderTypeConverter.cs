using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddOrderTypeConverter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderType",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Unknown",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderType",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Unknown");
        }
    }
}
