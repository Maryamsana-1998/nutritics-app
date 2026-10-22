using CafeKitchen.Domain.Recipes;

namespace CafeKitchen.Domain.Menus;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; }
    public List<MenuSection> Sections { get; set; } = [];
}

public class MenuSection
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }
    public List<MenuEntry> Entries { get; set; } = [];
}

public class MenuEntry
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    public bool IsSpecial { get; set; }
    public int SortOrder { get; set; }
}
