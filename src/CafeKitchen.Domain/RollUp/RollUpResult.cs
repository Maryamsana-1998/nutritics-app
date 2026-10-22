using CafeKitchen.Domain.Ingredients;

namespace CafeKitchen.Domain.RollUp;

public sealed record NutritionResult(
    Nutrition Per100g,
    Nutrition PerPortion,
    IReadOnlyList<string> MissingIngredients)
{
    public bool IsComplete => MissingIngredients.Count == 0;
}

public sealed record RollUpResult(
    Allergen Allergens,
    bool AllergensConfirmed,
    NutritionResult Nutrition,
    decimal BatchCost,
    decimal PortionCost,
    bool UsesDiscontinued);
