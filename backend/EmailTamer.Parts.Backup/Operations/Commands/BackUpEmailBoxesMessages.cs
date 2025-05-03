using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Parts.Sync.Persistence;
using EmailTamer.Parts.Sync.Services;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Commands;

public sealed record BackUpEmailBoxesMessages(Guid[]? EmailBoxesIds) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<BackUpEmailBoxesMessages>;
}

[UsedImplicitly]
public class BackUpEmailBoxesMessagesCommandHandler(
    IBackupService backupService,
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    ITenantRepository filesRepository)
    : IRequestHandler<BackUpEmailBoxesMessages, IActionResult>
{
    public async Task<IActionResult> Handle(BackUpEmailBoxesMessages request, CancellationToken cancellationToken)
    {
        return await backupService.BackupTenantEmailBoxesAsync(
            request.EmailBoxesIds,
            repository,
            filesRepository,
            cancellationToken);
    }
}