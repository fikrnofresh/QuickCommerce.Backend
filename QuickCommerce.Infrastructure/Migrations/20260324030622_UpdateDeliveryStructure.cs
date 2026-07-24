using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    public partial class UpdateDeliveryStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // ⚠️ NO RENAME
            // ⚠️ NO DROP
            // ONLY SAFE ADD
            // =========================

            // ADD agenttype if not exists
            migrationBuilder.AddColumn<int>(
                name: "agenttype",
                table: "deliveries",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            // ADD assignedtouserid
            migrationBuilder.AddColumn<int>(
                name: "assignedtouserid",
                table: "deliveries",
                type: "integer",
                nullable: true);

            // INDEX
            migrationBuilder.CreateIndex(
                name: "ix_deliveries_assignedtouserid",
                table: "deliveries",
                column: "assignedtouserid");

            // FK → users
            migrationBuilder.AddForeignKey(
                name: "fk_deliveries_users_assignedtouserid",
                table: "deliveries",
                column: "assignedtouserid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_deliveries_users_assignedtouserid",
                table: "deliveries");

            migrationBuilder.DropIndex(
                name: "ix_deliveries_assignedtouserid",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "agenttype",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "assignedtouserid",
                table: "deliveries");
        }
    }
} 