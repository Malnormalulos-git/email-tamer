using AutoMapper;
using EmailTamer.Core.Extensions;
using EmailTamer.Core.Startup;
using JetBrains.Annotations;

namespace EmailTamer.Startup;

[UsedImplicitly]
public class AutoMapperValidationAction(IMapper mapper, ILogger<AutoMapperValidationAction> logger)
    : IAsyncStartupAction
{
    public uint Order => 0;

    public Task PerformActionAsync(CancellationToken cancellationToken = default)
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
        return logger.TimeAsync(() =>
        {
            ((MapperConfiguration)mapper.ConfigurationProvider).CompileMappings();
            return Task.CompletedTask;
        }, "Compile AutoMapper mappings");
    }
}