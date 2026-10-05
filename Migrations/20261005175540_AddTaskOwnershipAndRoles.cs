using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskOwnershipAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM tasks) THEN
                        RAISE EXCEPTION 'Task ownership migration requires an empty tasks table. Assign existing tasks to users before retrying.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE users SET \"Role\" = 'User' WHERE \"Role\" IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "tasks",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_tasks_UserId",
                table: "tasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_users_UserId",
                table: "tasks",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_users_UserId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_UserId",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tasks");
        }
    }
}
