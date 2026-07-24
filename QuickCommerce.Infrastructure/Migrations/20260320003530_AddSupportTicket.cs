using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reference_id",
                table: "wallet_transaction");

            migrationBuilder.RenameColumn(
                name: "wallet_id",
                table: "wallet_transaction",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "transaction_type",
                table: "wallet_transaction",
                newName: "type");

            migrationBuilder.AddColumn<string>(
                name: "reference",
                table: "wallet_transaction",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reference",
                table: "wallet_transaction");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "wallet_transaction",
                newName: "wallet_id");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "wallet_transaction",
                newName: "transaction_type");

            migrationBuilder.AddColumn<string>(
                name: "reference_id",
                table: "wallet_transaction",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
