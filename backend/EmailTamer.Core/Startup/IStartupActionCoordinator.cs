namespace EmailTamer.Core.Startup;

public interface IStartupActionCoordinator
{
	Task PerformStartupActionsAsync(CancellationToken cancellationToken);
}