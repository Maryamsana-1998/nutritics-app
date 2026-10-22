using CafeKitchen.Domain.Ingredients;
using CafeKitchen.Domain.Menus;
using CafeKitchen.Domain.Recipes;

namespace CafeKitchen.Application.Abstractions;

public interface IIngredientRepository
{
    Task<IReadOnlyList<Ingredient>> SearchAsync(string? term, CancellationToken ct = default);
    Task<Ingredient?> GetAsync(int id, CancellationToken ct = default);
    Task AddAsync(Ingredient ingredient, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HeldBackRow>> GetPendingHeldBackAsync(CancellationToken ct = default);
}

public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken ct = default);

    /// <summary>Gets a recipe with lines, ingredients and child recipes loaded to any depth.</summary>
    Task<Recipe?> GetWithTreeAsync(int id, CancellationToken ct = default);

    /// <summary>Gets every recipe with its tree loaded, for roll-ups across the whole café.</summary>
    Task<IReadOnlyList<Recipe>> ListWithTreesAsync(CancellationToken ct = default);

    /// <summary>Child recipe ids used directly by each recipe, keyed by recipe id.</summary>
    Task<IReadOnlyDictionary<int, int[]>> GetChildMapAsync(CancellationToken ct = default);

    Task AddAsync(Recipe recipe, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IMenuRepository
{
    Task<IReadOnlyList<Menu>> ListAsync(CancellationToken ct = default);

    /// <summary>Active menus with sections, entries and full recipe trees loaded.</summary>
    Task<IReadOnlyList<Menu>> ListActiveWithDishesAsync(CancellationToken ct = default);

    Task AddAsync(Menu menu, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
