namespace CafeKitchen.Domain.Ingredients;

/// <summary>The 14 allergens UK/EU law requires to be declared.</summary>
[Flags]
public enum Allergen
{
    None = 0,
    Celery = 1 << 0,
    CerealsContainingGluten = 1 << 1,
    Crustaceans = 1 << 2,
    Eggs = 1 << 3,
    Fish = 1 << 4,
    Lupin = 1 << 5,
    Milk = 1 << 6,
    Molluscs = 1 << 7,
    Mustard = 1 << 8,
    TreeNuts = 1 << 9,
    Peanuts = 1 << 10,
    Sesame = 1 << 11,
    Soya = 1 << 12,
    SulphurDioxideAndSulphites = 1 << 13,
}
