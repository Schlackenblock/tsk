﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsk.HttpApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
