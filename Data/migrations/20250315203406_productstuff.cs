using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.migrations
{
    /// <inheritdoc />
    public partial class productstuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketProducts_Baskets_BasketId",
                table: "BasketProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Baskets_BasketId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Users_BasketId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "BasketId",
                table: "BasketProducts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts",
                newName: "IX_BasketProducts_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProducts_Users_UserId",
                table: "BasketProducts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketProducts_Users_UserId",
                table: "BasketProducts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BasketProducts",
                newName: "BasketId");

            migrationBuilder.RenameIndex(
                name: "IX_BasketProducts_UserId",
                table: "BasketProducts",
                newName: "IX_BasketProducts_BasketId");

            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_BasketId",
                table: "Users",
                column: "BasketId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProducts_Baskets_BasketId",
                table: "BasketProducts",
                column: "BasketId",
                principalTable: "Baskets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Baskets_BasketId",
                table: "Users",
                column: "BasketId",
                principalTable: "Baskets",
                principalColumn: "Id");
        }
    }
}
