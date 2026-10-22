using CafeKitchen.Domain.Common;
using CafeKitchen.Domain.Ingredients;

namespace CafeKitchen.Domain.Recipes;

/// <summary>A line references either an ingredient or a child recipe, never both.</summary>
public class RecipeLine
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int? IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }
    public int? ChildRecipeId { get; set; }
    public Recipe? ChildRecipe { get; set; }
    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
}
