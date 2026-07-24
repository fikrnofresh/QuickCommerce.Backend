using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InventoryMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventory_movements_store_storeid",
                table: "inventory_movements");

            migrationBuilder.RenameColumn(
                name: "storeid",
                table: "inventory_movements",
                newName: "store_id");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_movements_storeid",
                table: "inventory_movements",
                newName: "ix_inventory_movements_store_id");

            migrationBuilder.AlterColumn<string>(
                name: "reason",
                table: "inventory_movements",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "movement_type",
                table: "inventory_movements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "performed_by_user_id",
                table: "inventory_movements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "store_product_id",
                table: "inventory_movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_movements_store_product_id",
                table: "inventory_movements",
                column: "store_product_id");

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_movements_store_store_id",
                table: "inventory_movements",
                column: "store_id",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_movements_storeproducts_store_product_id",
                table: "inventory_movements",
                column: "store_product_id",
                principalTable: "storeproducts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventory_movements_store_store_id",
                table: "inventory_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_inventory_movements_storeproducts_store_product_id",
                table: "inventory_movements");

            migrationBuilder.DropIndex(
                name: "ix_inventory_movements_store_product_id",
                table: "inventory_movements");

            migrationBuilder.DropColumn(
                name: "movement_type",
                table: "inventory_movements");

            migrationBuilder.DropColumn(
                name: "performed_by_user_id",
                table: "inventory_movements");

            migrationBuilder.DropColumn(
                name: "store_product_id",
                table: "inventory_movements");

            migrationBuilder.RenameColumn(
                name: "store_id",
                table: "inventory_movements",
                newName: "storeid");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_movements_store_id",
                table: "inventory_movements",
                newName: "ix_inventory_movements_storeid");

            migrationBuilder.AlterColumn<string>(
                name: "reason",
                table: "inventory_movements",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_movements_store_storeid",
                table: "inventory_movements",
                column: "storeid",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
