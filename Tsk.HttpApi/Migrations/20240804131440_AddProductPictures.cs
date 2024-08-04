using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations
{
    public partial class AddProductPictures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "pictures",
                table: "products",
                type: "text[]",
                nullable: true
            );

            migrationBuilder.Sql(
                """
                UPDATE products
                SET pictures = '{}';
                """
            );

            migrationBuilder.AlterColumn<List<string>>(
                name: "pictures",
                table: "products",
                oldNullable: true,
                nullable: false
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pictures",
                table: "products"
            );
        }
    }
}
