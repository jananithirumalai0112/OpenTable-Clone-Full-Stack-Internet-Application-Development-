using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenTableApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CartItems_RestaurantId",
                table: "CartItems",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Restaurants_RestaurantId",
                table: "CartItems",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Restaurants_RestaurantId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_RestaurantId",
                table: "CartItems");
        }
    }
}
