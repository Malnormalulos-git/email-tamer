using EmailTamer.Core.Config;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmailTamer.Core.FluentValidation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsWithValidator<TOptions, TValidator>(this IServiceCollection services,
        string configurationSection)
        where TOptions : class, IAppConfig
        where TValidator : class, IValidator<TOptions>
    {
        services.TryAddTransient<IValidator<TOptions>, TValidator>();
        services.TryAddSingleton<IValidateOptions<TOptions>>(x
            => new OptionsFluentValidator<TOptions>(configurationSection,
                x.GetRequiredService<IValidator<TOptions>>(),
                x.GetRequiredService<IHostEnvironment>(),
                x.GetRequiredService<ILogger<OptionsFluentValidator<TOptions>>>()));
        services.AddOptions<TOptions>()
            .BindConfiguration(configurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}