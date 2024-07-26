using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations
{
    public partial class AddProductCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "products",
                type: "text",
                nullable: true
            );

            migrationBuilder.Sql(@"UPDATE ""products"" SET ""code"" = ""id"";");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "products",
                oldNullable: true,
                nullable: false
            );

            migrationBuilder.CreateIndex(
                name: "ix_products_code",
                table: "products",
                column: "code",
                unique: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_products_code",
                table: "products"
            );

            migrationBuilder.DropColumn(
                name: "code",
                table: "products"
            );
        }
    }
}
