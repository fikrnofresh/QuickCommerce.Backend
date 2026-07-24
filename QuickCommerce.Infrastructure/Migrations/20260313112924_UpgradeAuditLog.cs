using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_activity_log_user_user_id",
                table: "activity_log");

            migrationBuilder.DropIndex(
                name: "ix_activity_log_user_id",
                table: "activity_log");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "activity_log");

            migrationBuilder.AlterColumn<string>(
                name: "entity_name",
                table: "activity_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "activity_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "entity_id",
                table: "activity_log",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                table: "activity_log",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                table: "activity_log",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "module",
                table: "activity_log",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "entity_id",
                table: "activity_log");

            migrationBuilder.DropColumn(
                name: "ip_address",
                table: "activity_log");

            migrationBuilder.DropColumn(
                name: "metadata",
                table: "activity_log");

            migrationBuilder.DropColumn(
                name: "module",
                table: "activity_log");

            migrationBuilder.AlterColumn<string>(
                name: "entity_name",
                table: "activity_log",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "activity_log",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "activity_log",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_activity_log_user_id",
                table: "activity_log",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_activity_log_user_user_id",
                table: "activity_log",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
