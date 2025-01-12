using EmailTamer.Database.Entities;
using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database;

public class EmailTamerDbContext(
    DbContextOptions<EmailTamerDbContext> options,
    IDatabaseConfigurator configurator)
    : IdentityDbContext<EmailTamerUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        configurator.OnModelCreating(modelBuilder);

        modelBuilder.SeedRoles();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurator.ConfigureConventions(configurationBuilder, Database);
    }
}