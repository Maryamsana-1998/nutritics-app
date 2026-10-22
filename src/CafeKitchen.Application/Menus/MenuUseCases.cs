using CafeKitchen.Application.Abstractions;
using CafeKitchen.Domain.Ingredients;
using CafeKitchen.Domain.Menus;
using CafeKitchen.Domain.RollUp;

namespace CafeKitchen.Application.Menus;

/// <summary>Outcome of the allergy filter for one dish (B5). Never "safe" or "free from".</summary>
public enum FilterOutcome
{
    Contains,
    NotConfirmed,
    NoneDeclared,
}

public static class AllergyFilter
{
    public static FilterOutcome Evaluate(RollUpResult rollUp, Allergen ticked)
    {
        if ((rollUp.Allergens & ticked) != Allergen.None) return FilterOutcome.Contains;
        return rollUp.AllergensConfirmed ? FilterOutcome.NoneDeclared : FilterOutcome.NotConfirmed;
    }
}

public sealed record PublicDish(string Name, bool IsSpecial, RollUpResult RollUp, FilterOutcome Outcome);
public sealed record PublicSection(string Name, IReadOnlyList<PublicDish> Dishes);
public sealed record PublicMenu(string Name, IReadOnlyList<PublicSection> Sections);

public sealed class GetPublicMenus(IMenuRepository repository, RecipeRollUpCalculator calculator)
{
    public async Task<IReadOnlyList<PublicMenu>> ExecuteAsync(Allergen ticked, CancellationToken ct = default)
    {
        var menus = await repository.ListActiveWithDishesAsync(ct);
        return menus.Select(m => new PublicMenu(m.Name,
            m.Sections.OrderBy(s => s.SortOrder).Select(s => new PublicSection(s.Name,
                s.Entries.Where(e => e.Recipe is not null).OrderBy(e => e.SortOrder).Select(e =>
                {
                    var rollUp = calculator.Calculate(e.Recipe!);
                    return new PublicDish(e.Recipe!.Name, e.IsSpecial, rollUp, AllergyFilter.Evaluate(rollUp, ticked));
                }).ToList())).ToList())).ToList();
    }
}

public sealed class ListMenus(IMenuRepository repository)
{
    public Task<IReadOnlyList<Menu>> ExecuteAsync(CancellationToken ct = default) => repository.ListAsync(ct);
}

public sealed class SetMenuActive(IMenuRepository repository)
{
    public async Task ExecuteAsync(int menuId, bool isActive, CancellationToken ct = default)
    {
        var menu = (await repository.ListAsync(ct)).FirstOrDefault(m => m.Id == menuId)
            ?? throw new ValidationException("Menu not found.");
        menu.IsActive = isActive;
        await repository.SaveChangesAsync(ct);
    }
}
