using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreIdToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_store_store_id",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "store_id",
                table: "orders",
                newName: "storeid");

            migrationBuilder.RenameIndex(
                name: "ix_orders_store_id",
                table: "orders",
                newName: "ix_orders_storeid");

            migrationBuilder.AlterColumn<int>(
                name: "storeid",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_store_storeid",
                table: "orders",
                column: "storeid",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_store_storeid",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "storeid",
                table: "orders",
                newName: "store_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_storeid",
                table: "orders",
                newName: "ix_orders_store_id");

            migrationBuilder.AlterColumn<int>(
                name: "store_id",
                table: "orders",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_store_store_id",
                table: "orders",
                column: "store_id",
                principalTable: "store",
                principalColumn: "id");
        }
    }
}
