using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.migrations
{
    /// <inheritdoc />
    public partial class pasdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketProduct_Baskets_BasketId",
                table: "BasketProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_BasketProduct_Products_ProductId1",
                table: "BasketProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BasketProduct",
                table: "BasketProduct");

            migrationBuilder.DropIndex(
                name: "IX_BasketProduct_ProductId1",
                table: "BasketProduct");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "BasketProduct");

            migrationBuilder.RenameTable(
                name: "BasketProduct",
                newName: "BasketProducts");

            migrationBuilder.RenameIndex(
                name: "IX_BasketProduct_BasketId",
                table: "BasketProducts",
                newName: "IX_BasketProducts_BasketId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "BasketProducts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "BasketId",
                table: "BasketProducts",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "BasketProducts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BasketProducts",
                table: "BasketProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_ProductId",
                table: "BasketProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProducts_Baskets_BasketId",
                table: "BasketProducts",
                column: "BasketId",
                principalTable: "Baskets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProducts_Products_ProductId",
                table: "BasketProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketProducts_Baskets_BasketId",
                table: "BasketProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_BasketProducts_Products_ProductId",
                table: "BasketProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BasketProducts",
                table: "BasketProducts");

            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_ProductId",
                table: "BasketProducts");

            migrationBuilder.RenameTable(
                name: "BasketProducts",
                newName: "BasketProduct");

            migrationBuilder.RenameIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProduct",
                newName: "IX_BasketProduct_BasketId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "BasketProduct",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "BasketId",
                table: "BasketProduct",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "BasketProduct",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "ProductId1",
                table: "BasketProduct",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BasketProduct",
                table: "BasketProduct",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProduct_ProductId1",
                table: "BasketProduct",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProduct_Baskets_BasketId",
                table: "BasketProduct",
                column: "BasketId",
                principalTable: "Baskets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProduct_Products_ProductId1",
                table: "BasketProduct",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
