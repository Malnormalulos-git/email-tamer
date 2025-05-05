using EmailTamer.Core.DependencyInjection;
using EmailTamer.Core.FluentValidation;
using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Persistence.Interceptors;
using EmailTamer.Database.Services;
using EmailTamer.Database.Tenant.Accessor;
using EmailTamer.Database.Tenant.Config;
using EmailTamer.Database.Tenant.Services;
using EmailTamer.Database.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EmailTamer.Database.Tenant;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantDatabases(this IServiceCollection services)
    {
        services.AddDatabaseSaveChangesInterceptor<AuditSaveChangesInterceptor>();

        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();

        services.AddScoped<IDbContextFactory<TenantDbContext>>(sp =>
        {
            var options = new DbContextOptionsBuilder<TenantDbContext>();

            var hostEnv = sp.GetRequiredService<IHostEnvironment>();
            if (hostEnv.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }

            options.AddInterceptors(sp.GetServices<IOrderedInterceptor>().OrderBy(x => x.Order));

            return new TenantDbContextFactory(options, sp);
        });

        services.AddSharedDatabaseServices();

        return services;
    }

    private static IServiceCollection AddSharedDatabaseServices(this IServiceCollection services)
    {
        var assembly = typeof(TenantsDatabaseConfig).Assembly;
        services.AddCoreServicesFromAssembly(assembly);

        services.AddDatabaseUtilities();
        services.AddDatabasePersistence();
        services.AddDatabaseConfiguration<TenantsDatabaseConfig>();
        services.AddOptionsWithValidator<TenantsDatabaseConfig, TenantsDatabaseConfig.Validator>("TenantsDatabase");

        services.AddScoped<IEncryptionService, EncryptionService>();

        return services;
    }

    private static IServiceCollection AddDatabaseUtilities(this IServiceCollection services)
    {
        services.TryAddScoped<IDatabasePolicySet, DatabasePolicySet>();
        return services;
    }

    private static IServiceCollection AddDatabasePersistence(this IServiceCollection services)
    {
        services.TryAddKeyedScoped<IEmailTamerRepository>(nameof(TenantDbContext), (sp, _) =>
        {
            var dbContextFactory = sp.GetRequiredService<IDbContextFactory<TenantDbContext>>();
            var dbContext = dbContextFactory.CreateDbContext();

            var databasePolicySet = sp.GetRequiredService<IDatabasePolicySet>();

            var repository = new EmailTamerRepository<TenantDbContext>(dbContext, databasePolicySet);
            return repository;
        });

        return services;
    }
}