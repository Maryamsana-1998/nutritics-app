using CafeKitchen.Domain.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeKitchen.Infrastructure.Persistence.Configurations;

internal sealed class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> b)
    {
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.HasMany(x => x.Sections).WithOne().HasForeignKey(x => x.MenuId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class MenuSectionConfiguration : IEntityTypeConfiguration<MenuSection>
{
    public void Configure(EntityTypeBuilder<MenuSection> b)
    {
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.HasMany(x => x.Entries).WithOne().HasForeignKey(x => x.SectionId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class MenuEntryConfiguration : IEntityTypeConfiguration<MenuEntry>
{
    public void Configure(EntityTypeBuilder<MenuEntry> b) =>
        b.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
}
