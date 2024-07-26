using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations
{
    public partial class FixProductsPKName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""products"" RENAME CONSTRAINT ""PK_products"" TO ""pk_products"";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""products"" RENAME CONSTRAINT ""pk_products"" TO ""PK_products"";");
        }
    }
}
