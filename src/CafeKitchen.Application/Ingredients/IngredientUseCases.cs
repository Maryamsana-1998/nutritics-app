using CafeKitchen.Application.Abstractions;
using CafeKitchen.Domain.Common;
using CafeKitchen.Domain.Ingredients;

namespace CafeKitchen.Application.Ingredients;

public sealed class SearchIngredients(IIngredientRepository repository)
{
    public Task<IReadOnlyList<Ingredient>> ExecuteAsync(string? term, CancellationToken ct = default) =>
        repository.SearchAsync(term, ct);
}

public sealed class SaveIngredient(IIngredientRepository repository)
{
    public async Task ExecuteAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ingredient.Name)) throw new ValidationException("Name is required.");
        if (ingredient.PackSize <= 0) throw new ValidationException("Pack size must be greater than zero.");
        if (ingredient.PackPrice < 0) throw new ValidationException("Pack price cannot be negative.");
        if (ingredient.Unit == Unit.Each && ingredient.WeightPerUnitGrams is not > 0)
            throw new ValidationException("Items sold each need a weight per unit.");
        if (ingredient.AllergenStatus == AllergenStatus.NotDeclared) ingredient.Allergens = Allergen.None;

        if (ingredient.Id == 0) await repository.AddAsync(ingredient, ct);
        await repository.SaveChangesAsync(ct);
    }
}
