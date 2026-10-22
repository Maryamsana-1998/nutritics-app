using CafeKitchen.Application.Abstractions;
using CafeKitchen.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CafeKitchen.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CafeKitchenDbContext>(o =>
            o.UseSqlServer(configuration.GetConnectionString("CafeKitchen")));
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        return services;
    }
}
