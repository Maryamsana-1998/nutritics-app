using CafeKitchen.Domain.Ingredients;
using CafeKitchen.Domain.Menus;
using CafeKitchen.Domain.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CafeKitchen.Infrastructure.Persistence;

public class CafeKitchenDbContext(DbContextOptions<CafeKitchenDbContext> options) : DbContext(options)
{
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<HeldBackRow> HeldBackRows => Set<HeldBackRow>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeLine> RecipeLines => Set<RecipeLine>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuSection> MenuSections => Set<MenuSection>();
    public DbSet<MenuEntry> MenuEntries => Set<MenuEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CafeKitchenDbContext).Assembly);
}
