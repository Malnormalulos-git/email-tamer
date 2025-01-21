using EmailTamer.Database.Entities.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Tenant;

public class TenantDbContext(
    DbContextOptions<TenantDbContext> options,
    IDatabaseConfigurator configurator)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        configurator.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurator.ConfigureConventions(configurationBuilder, Database);
    }
}