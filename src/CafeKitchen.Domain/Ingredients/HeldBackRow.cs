namespace CafeKitchen.Domain.Ingredients;

public enum HeldBackStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
}

/// <summary>An import row whose allergens were kept because the file would remove one (B8).</summary>
public class HeldBackRow
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }
    public Allergen ProposedAllergens { get; set; }
    public HeldBackStatus Status { get; set; } = HeldBackStatus.Pending;
}
