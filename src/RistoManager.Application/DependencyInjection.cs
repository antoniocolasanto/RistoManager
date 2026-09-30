using Microsoft.Extensions.DependencyInjection;

namespace RistoManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Qui verranno registrati i servizi applicativi futuri
        return services;
    }
}