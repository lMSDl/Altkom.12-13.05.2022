using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescriptionId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Description",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Description", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_DescriptionId",
                table: "Order",
                column: "DescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Description_DescriptionId",
                table: "Order",
                column: "DescriptionId",
                principalTable: "Description",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Description_DescriptionId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "Description");

            migrationBuilder.DropIndex(
                name: "IX_Order_DescriptionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DescriptionId",
                table: "Order");
        }
    }
}
