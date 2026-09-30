using Microsoft.Extensions.DependencyInjection;

namespace RistoManager.Documents;

public static class DependencyInjection
{
    public static IServiceCollection AddDocuments(this IServiceCollection services)
    {
        // Predisposizione per i servizi documentali futuri
        return services;
    }
}