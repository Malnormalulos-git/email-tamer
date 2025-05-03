using EmailTamer.Database.Persistence;
using EmailTamer.Parts.Sync.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Parts.Sync.Services;

public interface IBackupService
{
    Task<IActionResult> BackupEmailBoxAsync(
        Guid emailBoxId,
        IEmailTamerRepository repository,
        ITenantRepository tenantRepository,
        CancellationToken cancellationToken);

    Task<IActionResult> BackupTenantEmailBoxesAsync(
        Guid[]? emailBoxesIds,
        IEmailTamerRepository repository,
        ITenantRepository tenantRepository,
        CancellationToken cancellationToken);
}