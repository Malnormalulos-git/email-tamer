using EmailTamer.Database.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Database;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDatabase(this IServiceCollection services, DatabaseConfig dbConfig)
	{
		services.AddDbContext<EmailTamerDbContext>(dbContextOptionsBuilder =>
			{
				dbContextOptionsBuilder
					.UseMySQL(dbConfig.ConnectionString, optionsBuilder => optionsBuilder
						.EnableRetryOnFailure(dbConfig.Retries)
						.CommandTimeout(dbConfig.Timeout));
			}, ServiceLifetime.Transient
		);

		return services;
	}
}