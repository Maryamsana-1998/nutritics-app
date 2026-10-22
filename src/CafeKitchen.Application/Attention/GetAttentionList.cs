using CafeKitchen.Application.Abstractions;
using CafeKitchen.Domain.Ingredients;
using CafeKitchen.Domain.Recipes;
using CafeKitchen.Domain.RollUp;

namespace CafeKitchen.Application.Attention;

public sealed record AttentionList(
    IReadOnlyList<Recipe> NotConfirmed,
    IReadOnlyList<Recipe> UsingDiscontinued,
    IReadOnlyList<HeldBackRow> HeldBack);

/// <summary>One screen of what needs action (F8).</summary>
public sealed class GetAttentionList(
    IRecipeRepository recipes,
    IIngredientRepository ingredients,
    RecipeRollUpCalculator calculator)
{
    public async Task<AttentionList> ExecuteAsync(CancellationToken ct = default)
    {
        var all = await recipes.ListWithTreesAsync(ct);
        var results = all.Select(r => (Recipe: r, RollUp: calculator.Calculate(r))).ToList();
        return new AttentionList(
            results.Where(x => !x.RollUp.AllergensConfirmed).Select(x => x.Recipe).ToList(),
            results.Where(x => x.RollUp.UsesDiscontinued).Select(x => x.Recipe).ToList(),
            await ingredients.GetPendingHeldBackAsync(ct));
    }
}
