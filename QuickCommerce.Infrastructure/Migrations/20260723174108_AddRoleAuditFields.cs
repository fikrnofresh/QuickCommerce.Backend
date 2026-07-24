using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuickCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "business_name",
                table: "store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gst_number",
                table: "store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "license_number",
                table: "store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pan_number",
                table: "store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "role",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "role",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "role",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "adminsessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    refreshtoken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    deviceinfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    operatingsystem = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ipaddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    logintime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    lastactivity = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    logouttime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    iscurrent = table.Column<bool>(type: "boolean", nullable: false),
                    isrevoked = table.Column<bool>(type: "boolean", nullable: false),
                    revokereason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_adminsessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_adminsessions_user_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passwordresettokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expiresat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    isused = table.Column<bool>(type: "boolean", nullable: false),
                    ipaddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    useragent = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    createdby = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_passwordresettokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_passwordresettokens_user_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_adminsessions_refreshtoken",
                table: "adminsessions",
                column: "refreshtoken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_adminsessions_userid",
                table: "adminsessions",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_passwordresettokens_token",
                table: "passwordresettokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_passwordresettokens_userid",
                table: "passwordresettokens",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adminsessions");

            migrationBuilder.DropTable(
                name: "passwordresettokens");

            migrationBuilder.DropColumn(
                name: "business_name",
                table: "store");

            migrationBuilder.DropColumn(
                name: "gst_number",
                table: "store");

            migrationBuilder.DropColumn(
                name: "license_number",
                table: "store");

            migrationBuilder.DropColumn(
                name: "pan_number",
                table: "store");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "role");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "role");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "role");
        }
    }
}
