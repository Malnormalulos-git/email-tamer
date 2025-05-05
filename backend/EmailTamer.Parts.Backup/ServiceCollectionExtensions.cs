using EmailTamer.Parts.Sync.Controllers;
using EmailTamer.Parts.Sync.Persistence;
using EmailTamer.Parts.Sync.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmailTamer.Parts.Sync;

public static class ServiceCollectionExtensions
{
    public static IMvcCoreBuilder AddBackupPart(this IMvcCoreBuilder builder)
    {
        builder.AddEmailTamerPart<BackupsController>();

        var services = builder.Services;

        services.AddScoped<ITenantRepositoryFactory, TenantRepositoryFactory>();

        services.TryAddScoped<ITenantRepository>(sp =>
            sp.GetRequiredService<ITenantRepositoryFactory>()
                .Create(default)
                .GetAwaiter()
                .GetResult());

        services.AddScoped<IBackupService, BackupService>();
        services.AddHostedService<PeriodicBackupService>();

        return builder;
    }
}