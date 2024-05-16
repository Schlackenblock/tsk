using Microsoft.EntityFrameworkCore.Migrations;

namespace Tsk.HttpApi.Migrations;

public partial class RemoveProductDescription : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "description",
            table: "products"
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "description",
            table: "products",
            type: "text",
            nullable: false,
            defaultValue: ""
        );
    }
}
