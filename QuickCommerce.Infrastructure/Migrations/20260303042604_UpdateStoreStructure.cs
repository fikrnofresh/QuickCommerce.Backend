using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fixedplatformfee",
                table: "stores");

            migrationBuilder.DropColumn(
                name: "issuspended",
                table: "stores");

            migrationBuilder.DropColumn(
                name: "platformfeepercentage",
                table: "stores");

            migrationBuilder.RenameColumn(
                name: "updatedat",
                table: "stores",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "phonenumber",
                table: "stores",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "openingtime",
                table: "stores",
                newName: "opening_time");

            migrationBuilder.RenameColumn(
                name: "isverified",
                table: "stores",
                newName: "is_verified");

            migrationBuilder.RenameColumn(
                name: "isonline",
                table: "stores",
                newName: "is_online");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "stores",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "stores",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "closingtime",
                table: "stores",
                newName: "closing_time");

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
                oldType: "character varying(20)",
                oldMaxLength: 20);

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

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "stores",
                newName: "updatedat");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "stores",
                newName: "phonenumber");

            migrationBuilder.RenameColumn(
                name: "opening_time",
                table: "stores",
                newName: "openingtime");

            migrationBuilder.RenameColumn(
                name: "is_verified",
                table: "stores",
                newName: "isverified");

            migrationBuilder.RenameColumn(
                name: "is_online",
                table: "stores",
                newName: "isonline");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "stores",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "stores",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "closing_time",
                table: "stores",
                newName: "closingtime");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "stores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "pincode",
                table: "stores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "stores",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "stores",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "stores",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "stores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "stores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "phonenumber",
                table: "stores",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "fixedplatformfee",
                table: "stores",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "issuspended",
                table: "stores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "platformfeepercentage",
                table: "stores",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
