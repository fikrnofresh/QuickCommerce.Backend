using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreStatusLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventory_movements_stores_storeid",
                table: "inventory_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_stores_storeid",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_storeproducts_stores_storeid",
                table: "storeproducts");

            migrationBuilder.DropForeignKey(
                name: "fk_user_stores_stores_store_id",
                table: "user_stores");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stores",
                table: "stores");

            migrationBuilder.RenameTable(
                name: "stores",
                newName: "store");

            migrationBuilder.RenameIndex(
                name: "ix_stores_code",
                table: "store",
                newName: "ix_store_code");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "pincode",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "store",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "store",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "store",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "store",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "store",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_store",
                table: "store",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_movements_store_storeid",
                table: "inventory_movements",
                column: "storeid",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_store_storeid",
                table: "orders",
                column: "storeid",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_storeproducts_store_storeid",
                table: "storeproducts",
                column: "storeid",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_stores_store_store_id",
                table: "user_stores",
                column: "store_id",
                principalTable: "store",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventory_movements_store_storeid",
                table: "inventory_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_store_storeid",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_storeproducts_store_storeid",
                table: "storeproducts");

            migrationBuilder.DropForeignKey(
                name: "fk_user_stores_store_store_id",
                table: "user_stores");

            migrationBuilder.DropPrimaryKey(
                name: "pk_store",
                table: "store");

            migrationBuilder.DropColumn(
                name: "status",
                table: "store");

            migrationBuilder.RenameTable(
                name: "store",
                newName: "stores");

            migrationBuilder.RenameIndex(
                name: "ix_store_code",
                table: "stores",
                newName: "ix_stores_code");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "stores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "pincode",
                table: "stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "stores",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "stores",
                type: "numeric(10,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "stores",
                type: "numeric(10,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "stores",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "stores",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "stores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "stores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stores",
                table: "stores",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_movements_stores_storeid",
                table: "inventory_movements",
                column: "storeid",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_stores_storeid",
                table: "orders",
                column: "storeid",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_storeproducts_stores_storeid",
                table: "storeproducts",
                column: "storeid",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_stores_stores_store_id",
                table: "user_stores",
                column: "store_id",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
