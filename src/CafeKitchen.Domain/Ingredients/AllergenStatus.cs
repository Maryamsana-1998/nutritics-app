namespace CafeKitchen.Domain.Ingredients;

/// <summary>
/// Whether an ingredient's allergens are known. An empty allergen list never means "none" (B1).
/// </summary>
public enum AllergenStatus
{
    NotDeclared = 0,
    Declared = 1,
}
