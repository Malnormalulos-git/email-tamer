using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Services;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Tenant;

public class TenantDbContext(
    DbContextOptions<TenantDbContext> options,
    IDatabaseConfigurator configurator,
    IEncryptionService? encryptionService)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        configurator.OnModelCreating(modelBuilder, encryptionService);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurator.ConfigureConventions(configurationBuilder, Database);
    }
}