using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations
{
    public partial class IntroduceCarts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => table.PrimaryKey("pk_carts", x => x.id)
            );

            migrationBuilder.CreateTable(
                name: "cart_products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    cart_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cart_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_cart_products_carts_cart_id",
                        column: x => x.cart_id,
                        principalTable: "carts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_cart_products_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                });

            migrationBuilder.CreateIndex(
                name: "ix_cart_products_cart_id",
                table: "cart_products",
                column: "cart_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_cart_products_product_id",
                table: "cart_products",
                column: "product_id"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("cart_products");
            migrationBuilder.DropTable("carts");
        }
    }
}
