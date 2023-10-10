using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace la_mia_pizzeria.Migrations
{
    /// <inheritdoc />
    public partial class modify_ingredients_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientPizza_pizzas_PizzaId",
                table: "IngredientPizza");

            migrationBuilder.RenameColumn(
                name: "PizzaId",
                table: "IngredientPizza",
                newName: "PizzasPizzaId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientPizza_PizzaId",
                table: "IngredientPizza",
                newName: "IX_IngredientPizza_PizzasPizzaId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientPizza_pizzas_PizzasPizzaId",
                table: "IngredientPizza",
                column: "PizzasPizzaId",
                principalTable: "pizzas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientPizza_pizzas_PizzasPizzaId",
                table: "IngredientPizza");

            migrationBuilder.RenameColumn(
                name: "PizzasPizzaId",
                table: "IngredientPizza",
                newName: "PizzaId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientPizza_PizzasPizzaId",
                table: "IngredientPizza",
                newName: "IX_IngredientPizza_PizzaId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientPizza_pizzas_PizzaId",
                table: "IngredientPizza",
                column: "PizzaId",
                principalTable: "pizzas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
