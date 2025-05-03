using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Parts.Sync.Persistence;
using EmailTamer.Parts.Sync.Services;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Commands;

public sealed record BackUpEmailBoxMessages(Guid EmailBoxId) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<BackUpEmailBoxMessages>
    {
        public Validator()
        {
            RuleFor(x => x.EmailBoxId).NotNull();
        }
    }
}

[UsedImplicitly]
internal class BackUpEmailBoxMessagesCommandHandler(
    IBackupService backupService,
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    ITenantRepository filesRepository)
    : IRequestHandler<BackUpEmailBoxMessages, IActionResult>
{
    public async Task<IActionResult> Handle(BackUpEmailBoxMessages command, CancellationToken cancellationToken)
    {
        return await backupService.BackupEmailBoxAsync(
            command.EmailBoxId,
            repository,
            filesRepository,
            cancellationToken);
    }
}