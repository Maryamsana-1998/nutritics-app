namespace CafeKitchen.Domain.Ingredients;

/// <summary>Nutrition per 100 g. Each nutrient is nullable so missing data is explicit (B4).</summary>
public sealed record Nutrition(
    decimal? EnergyKj = null,
    decimal? EnergyKcal = null,
    decimal? Fat = null,
    decimal? Saturates = null,
    decimal? Carbohydrate = null,
    decimal? Sugars = null,
    decimal? Fibre = null,
    decimal? Protein = null,
    decimal? Salt = null);
