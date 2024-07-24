using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations
{
    public partial class AddIsForSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_for_sale",
                table: "products",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.Sql(
                """
                UPDATE products
                SET is_for_sale = FALSE;
                """
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_for_sale",
                table: "products",
                oldNullable: true,
                nullable: false
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_for_sale",
                table: "products"
            );
        }
    }
}
