namespace CafeKitchen.Domain.Recipes;

/// <summary>One entity for both prep and dish; the role comes from how a recipe is used.</summary>
public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal YieldWeightGrams { get; set; }
    public int YieldPortions { get; set; } = 1;
    public decimal? SellingPrice { get; set; }
    public List<RecipeLine> Lines { get; set; } = [];

    public decimal? GrossProfit(decimal portionCost) => SellingPrice - portionCost;

    public decimal? MarginPercent(decimal portionCost) =>
        SellingPrice is > 0 ? (SellingPrice - portionCost) / SellingPrice * 100 : null;
}
