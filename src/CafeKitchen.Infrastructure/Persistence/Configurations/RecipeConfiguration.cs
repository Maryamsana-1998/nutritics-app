using CafeKitchen.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeKitchen.Infrastructure.Persistence.Configurations;

internal sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> b)
    {
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.YieldWeightGrams).HasPrecision(18, 4);
        b.Property(x => x.SellingPrice).HasPrecision(18, 2);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class RecipeLineConfiguration : IEntityTypeConfiguration<RecipeLine>
{
    public void Configure(EntityTypeBuilder<RecipeLine> b)
    {
        b.Property(x => x.Quantity).HasPrecision(18, 4);
        b.Property(x => x.Unit).HasConversion<string>().HasMaxLength(20);
        b.HasOne(x => x.Ingredient).WithMany().HasForeignKey(x => x.IngredientId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ChildRecipe).WithMany().HasForeignKey(x => x.ChildRecipeId).OnDelete(DeleteBehavior.Restrict);

        // A line references either an ingredient or a child recipe, never both.
        b.ToTable(t => t.HasCheckConstraint("CK_RecipeLine_OneTarget",
            "([IngredientId] IS NOT NULL AND [ChildRecipeId] IS NULL) OR ([IngredientId] IS NULL AND [ChildRecipeId] IS NOT NULL)"));
    }
}
