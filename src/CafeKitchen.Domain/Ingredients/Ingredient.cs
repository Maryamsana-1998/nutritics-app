using CafeKitchen.Domain.Common;

namespace CafeKitchen.Domain.Ingredients;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Supplier { get; set; } = "";
    public decimal PackSize { get; set; }
    public Unit Unit { get; set; }
    public decimal PackPrice { get; set; }

    /// <summary>Required when <see cref="Unit"/> is <see cref="Common.Unit.Each"/>.</summary>
    public decimal? WeightPerUnitGrams { get; set; }

    public AllergenStatus AllergenStatus { get; set; } = AllergenStatus.NotDeclared;
    public Allergen Allergens { get; set; } = Allergen.None;
    public Nutrition Nutrition { get; set; } = new();
    public bool IsDiscontinued { get; set; }

    /// <summary>Converts a quantity to grams (B3): ml counts as g, "each" uses the weight per unit.</summary>
    public decimal ToGrams(decimal quantity, Unit unit) => unit switch
    {
        Unit.Each => quantity * (WeightPerUnitGrams ?? 0),
        _ => quantity,
    };

    /// <summary>Pack size in grams.</summary>
    public decimal PackSizeGrams => ToGrams(PackSize, Unit);
}
