using EmailTamer.Database;
using EmailTamer.Database.Entities;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Accessor;
using EmailTamer.Database.Tenant.Services;
using EmailTamer.Database.Utilities;
using EmailTamer.Parts.Sync.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Parts.Sync.Services;

internal class PeriodicBackupService(
    ILogger<PeriodicBackupService> logger,
    IServiceScopeFactory factory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow.DateTime;
            var today = now.Date;
            var nextRun = today.AddHours(1);

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;

            logger.LogInformation("Next backup scheduled at {NextRun} UTC", nextRun);
            await Task.Delay(delay, stoppingToken);

            try
            {
                await using var mainScope = factory.CreateAsyncScope();
                var mainRepository = mainScope.ServiceProvider
                    .GetRequiredKeyedService<IEmailTamerRepository>(nameof(EmailTamerDbContext));

                var tenants = await mainRepository.ReadAsync((r, ct) =>
                        r.Set<EmailTamerUser>()
                            .AsNoTracking()
                            .Select(u => new TenantContextAccessor(u.TenantId))
                            .ToListAsync(ct),
                    stoppingToken);

                var backupTasks = tenants.Select(async tenant =>
                {
                    try
                    {
                        await using AsyncServiceScope scope = factory.CreateAsyncScope();
                        var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

                        var tenantRepositoryFactory = scope.ServiceProvider.GetRequiredService<ITenantRepositoryFactory>();
                        var tenantRepository = await tenantRepositoryFactory.Create(tenant, stoppingToken);

                        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TenantDbContext>>();
                        var databasePolicySet = scope.ServiceProvider.GetRequiredService<IDatabasePolicySet>();
                        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                        var encryptionService = new EncryptionService(tenant, configuration);
                        var dbContext = (dbContextFactory as TenantDbContextFactory)?.CreateDbContext(tenant, encryptionService);
                        var repository = new EmailTamerRepository<TenantDbContext>(dbContext, databasePolicySet);

                        var tenantId = await tenant.GetTenantId();
                        logger.LogInformation("Starting backup for tenant {TenantId}", tenantId);
                        var result = await backupService.BackupTenantEmailBoxesAsync(
                            null,
                            repository,
                            tenantRepository,
                            stoppingToken);
                        logger.LogInformation("Completed backup for tenant {TenantId}", tenantId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to backup tenant {TenantId}", await tenant.GetTenantId());
                    }
                });

                await Task.WhenAll(backupTasks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to execute PeriodicHostedService");
            }
        }
    }
}