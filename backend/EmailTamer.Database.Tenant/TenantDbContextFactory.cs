using EmailTamer.Database.Entities.Configuration;
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

        options.UseMySQL(connectionString,
            builder =>
            {
                var databaseConfig = serviceProvider.GetRequiredService<IOptionsMonitor<TenantsDatabaseConfig>>()
                    .CurrentValue;
                builder
                    .EnableRetryOnFailure(databaseConfig.Retries)
                    .CommandTimeout(databaseConfig.Timeout);
            });

        var configurator = serviceProvider.GetRequiredService<IDatabaseConfigurator>();
        
        var dbContext = new TenantDbContext(options.Options, configurator);

        dbContext.Database.Migrate();
        
        return dbContext;
    }

    private string GetConnectionStringForTenant()
    {
        var accessor = serviceProvider.GetRequiredService<ITenantContextAccessor>();
    
        var dbName = accessor.GetDatabaseName();

        var dbConfig = serviceProvider.GetRequiredService<IOptionsMonitor<TenantsDatabaseConfig>>().CurrentValue;

        var connectionString = string.Format(dbConfig.DefaultConnectionString, dbName);
        
        return connectionString;
    }
}