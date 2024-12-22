using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ProductImages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId1",
                table: "ProductImages",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId1",
                table: "ProductImages",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId1",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId1",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductImages");
        }
    }
}
