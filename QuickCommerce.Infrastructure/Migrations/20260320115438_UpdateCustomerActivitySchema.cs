using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerActivitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ SAFE: Only add score column (no jsonb conversion risk)
            migrationBuilder.AddColumn<decimal>(
                name: "score",
                table: "customer_activity",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "score",
                table: "customer_activity");
        }
    }
}