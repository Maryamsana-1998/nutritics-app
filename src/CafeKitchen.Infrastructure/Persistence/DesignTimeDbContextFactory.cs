using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CafeKitchen.Infrastructure.Persistence;

/// <summary>Lets `dotnet ef` create migrations without starting the web app.</summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CafeKitchenDbContext>
{
    public CafeKitchenDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CafeKitchenDbContext>()
            .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=CafeKitchen;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
        return new CafeKitchenDbContext(options);
    }
}
