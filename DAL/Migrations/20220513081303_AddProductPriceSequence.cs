using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddProductPriceSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sequences");

            migrationBuilder.CreateSequence<int>(
                name: "ProductPrice",
                schema: "sequences",
                startValue: 100L,
                incrementBy: 33,
                minValue: 10L,
                maxValue: 999L,
                cyclic: true);

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR sequences.ProductPrice",
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0.01f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "ProductPrice",
                schema: "sequences");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0.01f,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValueSql: "NEXT VALUE FOR sequences.ProductPrice");
        }
    }
}
