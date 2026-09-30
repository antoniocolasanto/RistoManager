using Microsoft.Extensions.DependencyInjection;

namespace RistoManager.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        // Qui verranno registrati i servizi di dominio futuri
        return services;
    }
}