using Microsoft.Extensions.DependencyInjection;

namespace RistoManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Predisposizione per EF Core e SQLite
        return services;
    }
}