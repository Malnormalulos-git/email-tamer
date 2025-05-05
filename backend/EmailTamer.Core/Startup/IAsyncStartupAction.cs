namespace EmailTamer.Core.Startup;

public interface IAsyncStartupAction
{
    uint Order { get; }

    Task PerformActionAsync(CancellationToken cancellationToken = default);
}