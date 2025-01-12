using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EmailTamer.Database.Entities.Configuration;

public interface IDatabaseConfigurator
{
	void OnModelCreating(ModelBuilder modelBuilder);

	void ConfigureConventions(ModelConfigurationBuilder modelConfigurationBuilder, DatabaseFacade databaseFacade);
}