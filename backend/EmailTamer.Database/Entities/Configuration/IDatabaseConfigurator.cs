using EmailTamer.Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EmailTamer.Database.Entities.Configuration;

public interface IDatabaseConfigurator
{
    void OnModelCreating(ModelBuilder modelBuilder, IEncryptionService? encryptionService = null);

    void ConfigureConventions(ModelConfigurationBuilder modelConfigurationBuilder, DatabaseFacade databaseFacade);
}