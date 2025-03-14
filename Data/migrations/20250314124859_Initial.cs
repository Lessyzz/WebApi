using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Universities",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Universities");

            migrationBuilder.RenameTable(
                name: "Universities",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Users",
                newName: "Roles");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JwtTokens",
                columns: table => new
                {
                    TokenJti = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JwtTokens", x => x.TokenJti);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JwtTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Universities");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Universities",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "Roles",
                table: "Universities",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Universities",
                newName: "Name");

            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "Universities",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "Universities",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Universities",
                table: "Universities",
                column: "Id");
        }
    }
}
