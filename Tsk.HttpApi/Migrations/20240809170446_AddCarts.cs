using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tsk.HttpApi.Migrations
{
    public partial class AddCarts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    products = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table => table.PrimaryKey("pk_carts", x => x.id)
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("carts");
        }
    }
}
