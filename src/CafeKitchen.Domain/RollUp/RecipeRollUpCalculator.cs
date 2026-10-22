using CafeKitchen.Domain.Common;
using CafeKitchen.Domain.Ingredients;
using CafeKitchen.Domain.Recipes;

namespace CafeKitchen.Domain.RollUp;

/// <summary>
/// Pure domain service that walks a recipe tree and rolls up allergens (B2), nutrition (B4),
/// cost (B6) and discontinued flags (B7). Recipes must have their lines, ingredients and child recipes loaded.
/// </summary>
public sealed class RecipeRollUpCalculator
{
    private const int NutrientCount = 9;

    private sealed class Totals
    {
        public Allergen Allergens;
        public bool Confirmed = true;
        public decimal Cost;
        public bool UsesDiscontinued;
        public decimal[] Nutrients = new decimal[NutrientCount];
        public HashSet<string> Missing = new(StringComparer.OrdinalIgnoreCase);
    }

    public RollUpResult Calculate(Recipe recipe)
    {
        var memo = new Dictionary<int, Totals>();
        var t = TotalsFor(recipe, memo, []);

        var yield = recipe.YieldWeightGrams;
        var portions = Math.Max(recipe.YieldPortions, 1);
        var per100 = yield > 0 ? Scale(t.Nutrients, 100m / yield) : new decimal[NutrientCount];
        var perPortion = Scale(t.Nutrients, 1m / portions);

        var missing = t.Missing.OrderBy(x => x).ToList();
        if (yield <= 0) missing.Add("Yield weight");

        return new RollUpResult(
            t.Allergens,
            t.Confirmed,
            new NutritionResult(ToNutrition(per100), ToNutrition(perPortion), missing),
            t.Cost,
            t.Cost / portions,
            t.UsesDiscontinued);
    }

    private Totals TotalsFor(Recipe recipe, Dictionary<int, Totals> memo, HashSet<int> path)
    {
        if (recipe.Id != 0 && memo.TryGetValue(recipe.Id, out var cached)) return cached;
        if (recipe.Id != 0 && !path.Add(recipe.Id))
            throw new InvalidOperationException($"Recipe '{recipe.Name}' contains itself.");

        var t = new Totals();
        foreach (var line in recipe.Lines)
        {
            if (line.Ingredient is { } ing) AddIngredient(t, ing, line);
            else if (line.ChildRecipe is { } child) AddChild(t, child, line, memo, path);
        }

        if (recipe.Id != 0)
        {
            path.Remove(recipe.Id);
            memo[recipe.Id] = t;
        }
        return t;
    }

    private static void AddIngredient(Totals t, Ingredient ing, RecipeLine line)
    {
        if (ing.AllergenStatus == AllergenStatus.NotDeclared) t.Confirmed = false;
        else t.Allergens |= ing.Allergens;
        if (ing.IsDiscontinued) t.UsesDiscontinued = true;

        var grams = ing.ToGrams(line.Quantity, line.Unit);

        if (line.Unit == ing.Unit && ing.PackSize > 0) t.Cost += line.Quantity / ing.PackSize * ing.PackPrice;
        else if (ing.PackSizeGrams > 0) t.Cost += grams / ing.PackSizeGrams * ing.PackPrice;

        var n = ing.Nutrition;
        decimal?[] values = [n.EnergyKj, n.EnergyKcal, n.Fat, n.Saturates, n.Carbohydrate, n.Sugars, n.Fibre, n.Protein, n.Salt];
        for (var i = 0; i < NutrientCount; i++)
        {
            if (values[i] is { } v) t.Nutrients[i] += v * grams / 100m;
            else t.Missing.Add(ing.Name);
        }
    }

    private void AddChild(Totals t, Recipe child, RecipeLine line, Dictionary<int, Totals> memo, HashSet<int> path)
    {
        var c = TotalsFor(child, memo, path);
        t.Allergens |= c.Allergens;
        if (!c.Confirmed) t.Confirmed = false;
        if (c.UsesDiscontinued) t.UsesDiscontinued = true;
        foreach (var m in c.Missing) t.Missing.Add(m);

        // "each" of a prep is one portion; otherwise ml counts as g (B3).
        var grams = line.Unit == Unit.Each
            ? line.Quantity * (child.YieldWeightGrams / Math.Max(child.YieldPortions, 1))
            : line.Quantity;
        var share = child.YieldWeightGrams > 0 ? grams / child.YieldWeightGrams : 0;

        t.Cost += c.Cost * share;
        for (var i = 0; i < NutrientCount; i++) t.Nutrients[i] += c.Nutrients[i] * share;
    }

    private static decimal[] Scale(decimal[] values, decimal factor) => values.Select(v => v * factor).ToArray();

    private static Nutrition ToNutrition(decimal[] v) =>
        new(v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8]);
}
