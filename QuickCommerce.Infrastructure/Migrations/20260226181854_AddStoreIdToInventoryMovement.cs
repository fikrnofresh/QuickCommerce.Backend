using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreIdToInventoryMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "performed_by_user_id",
                table: "inventory_movements");

            migrationBuilder.AddColumn<int>(
                name: "storeid",
                table: "inventory_movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_movements_storeid",
                table: "inventory_movements",
                column: "storeid");

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_movements_store_storeid",
                table: "inventory_movements",
                column: "storeid",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventory_movements_store_storeid",
                table: "inventory_movements");

            migrationBuilder.DropIndex(
                name: "ix_inventory_movements_storeid",
                table: "inventory_movements");

            migrationBuilder.DropColumn(
                name: "storeid",
                table: "inventory_movements");

            migrationBuilder.AddColumn<int>(
                name: "performed_by_user_id",
                table: "inventory_movements",
                type: "integer",
                nullable: true);
        }
    }
}
