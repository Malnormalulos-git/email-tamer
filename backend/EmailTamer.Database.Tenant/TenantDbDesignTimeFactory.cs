using System.Reflection;
using EmailTamer.Database.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Database.Tenant;

// This class exists only for creating TenantDbContext migrations

public class TenantDbDesignTimeFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();

        optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=emailtamer;Uid=emailtameruser;Pwd=emailtamerpassword;");

        var entityConfigurations = LoadEntityConfigurations();

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<DatabaseConfigurator>();

        var configurator = new DatabaseConfigurator(entityConfigurations, logger);

        return new TenantDbContext(optionsBuilder.Options, configurator);
    }
    
    private List<INonGenericEntityConfiguration> LoadEntityConfigurations()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var configurations = assembly.GetTypes()
            .Where(type => typeof(INonGenericEntityConfiguration).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<INonGenericEntityConfiguration>()
            .ToList();

        return configurations;
    }
}