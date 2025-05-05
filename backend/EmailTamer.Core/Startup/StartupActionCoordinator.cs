using EmailTamer.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Core.Startup;

internal sealed class StartupActionCoordinator(
    IServiceProvider serviceProvider,
    ILogger<StartupActionCoordinator> logger)
    : IStartupActionCoordinator
{
    public Task PerformStartupActionsAsync(CancellationToken cancellationToken)
        => logger.TimeAsync(async () =>
        {
            var actions = serviceProvider
                .GetServices<IAsyncStartupAction>()
                .OrderBy(x => x.Order)
                .ToList();

            if (actions.Count == 0)
            {
                return;
            }

            logger.LogDebug("Will run {StartupActionCount} startup actions: {@StartupActions}",
                actions.Count,
                actions.Select(a => new { a.GetType().Name, a.Order }));

            foreach (var actionGroup in actions.GroupBy(x => x.Order))
            {
                await logger.TimeAsync(() =>
                {
                    var tasks = actionGroup.Select(a => logger.TimeAsync(
                        () => a.PerformActionAsync(cancellationToken),
                        "run startup action '{StartupActionName}'", a.GetType().Name));
                    return Task.WhenAll(tasks);
                }, "run startup action group '{StartupActionGroup}'", actionGroup.Key);
            }
        }, "startup");
}