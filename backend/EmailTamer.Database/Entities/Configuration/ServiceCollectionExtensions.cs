using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmailTamer.Database.Entities.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseConfiguration<T>(this IServiceCollection services)
    {
        services.TryAddScoped<IDatabaseConfigurator, DatabaseConfigurator>();
        services.CollectDatabaseEntities<T>();
        return services;
    }

    private static IServiceCollection CollectDatabaseEntities<T>(this IServiceCollection services)
    {
        var entityConfigurationType = typeof(INonGenericEntityConfiguration);

        var configurations = typeof(T).Assembly.GetExportedTypes()
            .Where(x => x is { IsAbstract: false, IsClass: true } && x.IsAssignableTo(entityConfigurationType))
            .Select(c => new ServiceDescriptor(entityConfigurationType, c, ServiceLifetime.Scoped));
        services.TryAddEnumerable(configurations);

        return services;
    }
}