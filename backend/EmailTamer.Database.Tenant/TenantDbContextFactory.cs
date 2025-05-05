using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Services;
using EmailTamer.Database.Tenant.Accessor;
using EmailTamer.Database.Tenant.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EmailTamer.Database.Tenant;

public class TenantDbContextFactory(
    DbContextOptionsBuilder<TenantDbContext> options,
    IServiceProvider serviceProvider)
    : IDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext()
    {
        var connectionString = GetConnectionStringForTenant();
        return CreateDbContextInternal(connectionString);
    }

    public TenantDbContext CreateDbContext(ITenantContextAccessor tenant, IEncryptionService encryptionService)
    {
        var connectionString = GetConnectionString(tenant.GetDatabaseName());
        return CreateDbContextInternal(connectionString, encryptionService);
    }

    private TenantDbContext CreateDbContextInternal(string connectionString, IEncryptionService? encryptionService = null)
    {
        var dbConfig = serviceProvider.GetRequiredService<IOptionsMonitor<TenantsDatabaseConfig>>().CurrentValue;

        options.UseMySQL(connectionString,
            builder =>
            {
                builder
                    .EnableRetryOnFailure(dbConfig.Retries)
                    .CommandTimeout(dbConfig.Timeout);
            });

        var configurator = serviceProvider.GetRequiredService<IDatabaseConfigurator>();
        var encryptService = encryptionService ?? serviceProvider.GetRequiredService<IEncryptionService>();

        var dbContext = new TenantDbContext(options.Options, configurator, encryptService);
        dbContext.Database.Migrate();

        return dbContext;
    }

    private string GetConnectionString(string dbName)
    {
        var dbConfig = serviceProvider.GetRequiredService<IOptionsMonitor<TenantsDatabaseConfig>>().CurrentValue;
        return string.Format(dbConfig.DefaultConnectionString, dbName);
    }

    private string GetConnectionStringForTenant()
    {
        var accessor = serviceProvider.GetRequiredService<ITenantContextAccessor>();

        var dbName = accessor.GetDatabaseName();

        return GetConnectionString(dbName);
    }
}