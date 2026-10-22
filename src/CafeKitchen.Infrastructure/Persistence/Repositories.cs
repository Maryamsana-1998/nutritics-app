using CafeKitchen.Application.Abstractions;
using CafeKitchen.Domain.Ingredients;
using CafeKitchen.Domain.Menus;
using CafeKitchen.Domain.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CafeKitchen.Infrastructure.Persistence;

internal sealed class IngredientRepository(CafeKitchenDbContext db) : IIngredientRepository
{
    public async Task<IReadOnlyList<Ingredient>> SearchAsync(string? term, CancellationToken ct = default)
    {
        var query = db.Ingredients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(i => i.Name.Contains(term) || i.Supplier.Contains(term));
        return await query.OrderBy(i => i.Name).ToListAsync(ct);
    }

    public Task<Ingredient?> GetAsync(int id, CancellationToken ct = default) =>
        db.Ingredients.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task AddAsync(Ingredient ingredient, CancellationToken ct = default) =>
        await db.Ingredients.AddAsync(ingredient, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<HeldBackRow>> GetPendingHeldBackAsync(CancellationToken ct = default) =>
        await db.HeldBackRows.AsNoTracking().Include(h => h.Ingredient)
            .Where(h => h.Status == HeldBackStatus.Pending).ToListAsync(ct);
}

internal sealed class RecipeRepository(CafeKitchenDbContext db) : IRecipeRepository
{
    public async Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken ct = default) =>
        await db.Recipes.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);

    public async Task<Recipe?> GetWithTreeAsync(int id, CancellationToken ct = default)
    {
        // Loading every recipe and line lets EF fix up the child-recipe graph to any depth.
        await LoadAllAsync(ct);
        return await db.Recipes.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<IReadOnlyList<Recipe>> ListWithTreesAsync(CancellationToken ct = default)
    {
        await LoadAllAsync(ct);
        return await db.Recipes.OrderBy(r => r.Name).ToListAsync(ct);
    }

    public async Task<IReadOnlyDictionary<int, int[]>> GetChildMapAsync(CancellationToken ct = default) =>
        (await db.RecipeLines.AsNoTracking().Where(l => l.ChildRecipeId != null)
            .Select(l => new { l.RecipeId, Child = l.ChildRecipeId!.Value }).ToListAsync(ct))
        .GroupBy(x => x.RecipeId)
        .ToDictionary(g => g.Key, g => g.Select(x => x.Child).ToArray());

    public async Task AddAsync(Recipe recipe, CancellationToken ct = default) => await db.Recipes.AddAsync(recipe, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);

    private async Task LoadAllAsync(CancellationToken ct)
    {
        await db.Ingredients.LoadAsync(ct);
        await db.Recipes.LoadAsync(ct);
        await db.RecipeLines.LoadAsync(ct);
    }
}

internal sealed class MenuRepository(CafeKitchenDbContext db, IRecipeRepository recipes) : IMenuRepository
{
    public async Task<IReadOnlyList<Menu>> ListAsync(CancellationToken ct = default) =>
        await db.Menus.OrderBy(m => m.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Menu>> ListActiveWithDishesAsync(CancellationToken ct = default)
    {
        await recipes.ListWithTreesAsync(ct);
        return await db.Menus.Where(m => m.IsActive)
            .Include(m => m.Sections).ThenInclude(s => s.Entries).ThenInclude(e => e.Recipe)
            .OrderBy(m => m.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Menu menu, CancellationToken ct = default) => await db.Menus.AddAsync(menu, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
