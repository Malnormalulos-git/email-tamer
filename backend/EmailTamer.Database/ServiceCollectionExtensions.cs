using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using EmailTamer.Core.DependencyInjection;
using EmailTamer.Core.FluentValidation;
using EmailTamer.Core.Startup;
using EmailTamer.Database.Config;
using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Persistence.Interceptors;
using EmailTamer.Database.Startup;
using EmailTamer.Database.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EmailTamer.Database;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDatabase(this IServiceCollection services)
	{
		var assembly = typeof(DatabaseConfig).Assembly;

		services.AddStartupAction<RunDbMigrationsAction>();

		services.AddDatabaseSaveChangesInterceptor<AuditSaveChangesInterceptor>();

		services.AddHealthChecks()
			.AddMySql(
				sp =>
				{
					var databaseConfig = sp.GetRequiredService<IOptionsMonitor<DatabaseConfig>>()
						.CurrentValue;
					return databaseConfig.ConnectionString;
				},
				name: "database"
			);

		services.AddDbContext<EmailTamerDbContext>((sp, options) =>
			{
				var hostEnv = sp.GetRequiredService<IHostEnvironment>();
				if (hostEnv.IsDevelopment())
				{
					options.EnableSensitiveDataLogging()
						.EnableDetailedErrors();
				}

				var databaseConfig = sp.GetRequiredService<IOptionsMonitor<DatabaseConfig>>()
					.CurrentValue;

				options
					.UseMySQL(databaseConfig.ConnectionString,
						builder =>
						{
							builder
								.EnableRetryOnFailure(databaseConfig.Retries)
								.CommandTimeout(databaseConfig.Timeout);
						}
					);

				options.AddInterceptors(sp.GetServices<IOrderedInterceptor>().OrderBy(x => x.Order));
			}
		);

		services.AddSharedDatabaseServices();

		services.AddAutoMapper((provider, expression) =>
		{
			expression.AddExpressionMapping();
			expression.UseEntityFrameworkCoreModel<EmailTamerDbContext>(provider);
		}, assembly);

		return services;
	}

	private static IServiceCollection AddSharedDatabaseServices(this IServiceCollection services)
	{
		var assembly = typeof(DatabaseConfig).Assembly;
		services.AddCoreServicesFromAssembly(assembly);

		services.AddDatabaseUtilities();
		services.AddDatabasePersistence();
		services.AddDatabaseConfiguration<DatabaseConfig>();
		services.AddOptionsWithValidator<DatabaseConfig, DatabaseConfig.Validator>("Database");

		return services;
	}

	private static IServiceCollection AddDatabaseUtilities(this IServiceCollection services)
	{
		services.TryAddScoped<IDatabasePolicySet, DatabasePolicySet>();
		return services;
	}

	private static IServiceCollection AddDatabasePersistence(this IServiceCollection services)
	{
		services.TryAddKeyedScoped<IEmailTamerRepository>(nameof(EmailTamerDbContext), (sp, _) =>
		{
			var dbContext = sp.GetRequiredService<EmailTamerDbContext>();
			var databasePolicySet = sp.GetRequiredService<IDatabasePolicySet>();
			
			var repository = new EmailTamerRepository<EmailTamerDbContext>(dbContext, databasePolicySet);
			return repository;
		});
		
		return services;
	}
}