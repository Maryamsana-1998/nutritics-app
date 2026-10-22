using CafeKitchen.Application.Attention;
using CafeKitchen.Application.Ingredients;
using CafeKitchen.Application.Menus;
using CafeKitchen.Application.Recipes;
using CafeKitchen.Domain.RollUp;
using Microsoft.Extensions.DependencyInjection;

namespace CafeKitchen.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<RecipeRollUpCalculator>();
        services.AddScoped<SearchIngredients>();
        services.AddScoped<SaveIngredient>();
        services.AddScoped<ListRecipes>();
        services.AddScoped<GetRecipeRollUp>();
        services.AddScoped<SaveRecipe>();
        services.AddScoped<AddRecipeLine>();
        services.AddScoped<GetPublicMenus>();
        services.AddScoped<ListMenus>();
        services.AddScoped<SetMenuActive>();
        services.AddScoped<GetAttentionList>();
        return services;
    }
}
