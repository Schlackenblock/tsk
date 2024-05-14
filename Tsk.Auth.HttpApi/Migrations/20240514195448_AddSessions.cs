using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tsk.Auth.HttpApi.Migrations;

public partial class AddSessions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "sessions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                refresh_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_sessions_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade
                );
            }
        );

        migrationBuilder.CreateIndex(
            name: "ix_sessions_refresh_token_id",
            table: "sessions",
            column: "refresh_token_id"
        );

        migrationBuilder.CreateIndex(
            name: "ix_sessions_user_id",
            table: "sessions",
            column: "user_id"
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("sessions");
    }
}
