namespace CafeKitchen.Domain.Ingredients;

/// <summary>Import ratchet (B8): an import may add allergens but never remove one unaccepted.</summary>
public static class ImportRatchet
{
    public static bool WouldRemoveAllergen(Allergen current, Allergen proposed) => (current & ~proposed) != Allergen.None;
}
