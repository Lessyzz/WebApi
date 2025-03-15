using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.migrations
{
    /// <inheritdoc />
    public partial class Basket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasketProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    BasketId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketProduct_Basket_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Basket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_BasketId",
                table: "Users",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProduct_BasketId",
                table: "BasketProduct",
                column: "BasketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Basket_BasketId",
                table: "Users",
                column: "BasketId",
                principalTable: "Basket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Basket_BasketId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "BasketProduct");

            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropIndex(
                name: "IX_Users_BasketId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Users");
        }
    }
}
