using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    public partial class RefreshTokenFinalFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ❌ REMOVE ALL RenameColumn (WRONG)

            // ✅ ONLY KEEP REFRESH TOKEN CHANGES

            migrationBuilder.AlterColumn<string>(
                name: "replacedbytoken",
                table: "refreshtokens",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deviceinfo",
                table: "refreshtokens",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipaddress",
                table: "refreshtokens",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_refreshtokens_token",
                table: "refreshtokens",
                column: "token",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_refreshtokens_token",
                table: "refreshtokens");

            migrationBuilder.DropColumn(
                name: "deviceinfo",
                table: "refreshtokens");

            migrationBuilder.DropColumn(
                name: "ipaddress",
                table: "refreshtokens");

            migrationBuilder.AlterColumn<string>(
                name: "replacedbytoken",
                table: "refreshtokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}