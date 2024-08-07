using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations;

public partial class FixMeetupNaming : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_Products",
            table: "Products"
        );

        migrationBuilder.RenameTable(
            name: "Products",
            newName: "products"
        );

        migrationBuilder.RenameColumn(
            name: "Title",
            table: "products",
            newName: "title"
        );

        migrationBuilder.RenameColumn(
            name: "Price",
            table: "products",
            newName: "price"
        );

        migrationBuilder.RenameColumn(
            name: "Description",
            table: "products",
            newName: "description"
        );

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "products",
            newName: "id"
        );

        migrationBuilder.AddPrimaryKey(
            name: "PK_products",
            table: "products",
            column: "id"
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_products",
            table: "products"
        );

        migrationBuilder.RenameTable(
            name: "products",
            newName: "Products"
        );

        migrationBuilder.RenameColumn(
            name: "title",
            table: "Products",
            newName: "Title"
        );

        migrationBuilder.RenameColumn(
            name: "price",
            table: "Products",
            newName: "Price"
        );

        migrationBuilder.RenameColumn(
            name: "description",
            table: "Products",
            newName: "Description"
        );

        migrationBuilder.RenameColumn(
            name: "id",
            table: "Products",
            newName: "Id"
        );

        migrationBuilder.AddPrimaryKey(
            name: "PK_Products",
            table: "Products",
            column: "Id"
        );
    }
}
