using CafeKitchen.Domain.Ingredients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeKitchen.Infrastructure.Persistence.Configurations;

internal sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> b)
    {
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Supplier).HasMaxLength(200).IsRequired();
        b.Property(x => x.PackSize).HasPrecision(18, 4);
        b.Property(x => x.PackPrice).HasPrecision(18, 4);
        b.Property(x => x.WeightPerUnitGrams).HasPrecision(18, 4);
        b.Property(x => x.Unit).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.AllergenStatus).HasConversion<string>().HasMaxLength(20);
        b.Ignore(x => x.PackSizeGrams);
        b.HasIndex(x => new { x.Supplier, x.Name }).IsUnique();

        b.ComplexProperty(x => x.Nutrition, n =>
        {
            foreach (var p in new[]
            {
                nameof(Nutrition.EnergyKj), nameof(Nutrition.EnergyKcal), nameof(Nutrition.Fat),
                nameof(Nutrition.Saturates), nameof(Nutrition.Carbohydrate), nameof(Nutrition.Sugars),
                nameof(Nutrition.Fibre), nameof(Nutrition.Protein), nameof(Nutrition.Salt),
            })
                n.Property<decimal?>(p).HasPrecision(18, 4);
        });
    }
}

internal sealed class HeldBackRowConfiguration : IEntityTypeConfiguration<HeldBackRow>
{
    public void Configure(EntityTypeBuilder<HeldBackRow> b)
    {
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasOne(x => x.Ingredient).WithMany().HasForeignKey(x => x.IngredientId).OnDelete(DeleteBehavior.Cascade);
    }
}
