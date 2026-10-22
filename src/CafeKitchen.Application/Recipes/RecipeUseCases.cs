using CafeKitchen.Application.Abstractions;
using CafeKitchen.Domain.Recipes;
using CafeKitchen.Domain.RollUp;

namespace CafeKitchen.Application.Recipes;

public sealed record RecipeSummary(Recipe Recipe, RollUpResult RollUp);

public sealed class ListRecipes(IRecipeRepository repository, RecipeRollUpCalculator calculator)
{
    public async Task<IReadOnlyList<RecipeSummary>> ExecuteAsync(CancellationToken ct = default)
    {
        var recipes = await repository.ListWithTreesAsync(ct);
        return recipes.Select(r => new RecipeSummary(r, calculator.Calculate(r))).ToList();
    }
}

public sealed class GetRecipeRollUp(IRecipeRepository repository, RecipeRollUpCalculator calculator)
{
    public async Task<RecipeSummary?> ExecuteAsync(int recipeId, CancellationToken ct = default)
    {
        var recipe = await repository.GetWithTreeAsync(recipeId, ct);
        return recipe is null ? null : new RecipeSummary(recipe, calculator.Calculate(recipe));
    }
}

public sealed class SaveRecipe(IRecipeRepository repository)
{
    public async Task ExecuteAsync(Recipe recipe, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(recipe.Name)) throw new ValidationException("Name is required.");
        if (recipe.YieldPortions < 1) throw new ValidationException("A recipe needs at least one portion.");
        if (recipe.YieldWeightGrams < 0) throw new ValidationException("Yield weight cannot be negative.");

        if (recipe.Id == 0) await repository.AddAsync(recipe, ct);
        await repository.SaveChangesAsync(ct);
    }
}

public sealed class AddRecipeLine(IRecipeRepository repository)
{
    public async Task ExecuteAsync(int recipeId, RecipeLine line, CancellationToken ct = default)
    {
        if (line.Quantity <= 0) throw new ValidationException("Quantity must be greater than zero.");
        if ((line.IngredientId is null) == (line.ChildRecipeId is null))
            throw new ValidationException("A line must use either an ingredient or a recipe.");

        var recipe = await repository.GetWithTreeAsync(recipeId, ct)
            ?? throw new ValidationException("Recipe not found.");

        if (line.ChildRecipeId is { } childId)
        {
            var map = await repository.GetChildMapAsync(ct);
            if (RecipeCycleDetector.WouldCreateCycle(recipeId, childId,
                    id => map.TryGetValue(id, out var kids) ? kids : []))
                throw new ValidationException("A recipe cannot contain itself, directly or indirectly.");
        }

        line.RecipeId = recipeId;
        recipe.Lines.Add(line);
        await repository.SaveChangesAsync(ct);
    }
}
