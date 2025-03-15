using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.migrations
{
    /// <inheritdoc />
    public partial class fvgdfghfdsaadf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Basket_BasketId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "BasketProduct");

            migrationBuilder.DropIndex(
                name: "IX_Users_BasketId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Basket",
                table: "Basket");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Basket",
                newName: "Baskets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Baskets",
                table: "Baskets",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Baskets",
                table: "Baskets");

            migrationBuilder.RenameTable(
                name: "Baskets",
                newName: "Basket");

            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Basket",
                table: "Basket",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BasketProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BasketId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
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
    }
}
