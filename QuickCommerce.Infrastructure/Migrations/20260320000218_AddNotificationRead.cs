using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_read",
                table: "notifications");
        }
    }
}
