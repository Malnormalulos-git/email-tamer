using EmailTamer.Core.Config;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmailTamer.Core.FluentValidation;

internal sealed class OptionsFluentValidator<T>(
    string configurationSection,
    IValidator<T> validator,
    IHostEnvironment environment,
    ILogger<OptionsFluentValidator<T>> logger)
    : IValidateOptions<T>
    where T : class, IAppConfig
{
    public ValidateOptionsResult Validate(string? name, T options)
    {
        var result = validator.Validate(options);
        if (result.IsValid)
        {
            if (environment.IsDevelopment())
            {
                logger.LogDebug("Config section {ConfigSection} is valid, value {@Value}",
                    configurationSection, options);
            }
            else
            {
                logger.LogDebug("Config section {ConfigSection} is valid", configurationSection);
            }

            return ValidateOptionsResult.Success;
        }

        var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
        var errorMessage = $"Config section '{configurationSection}' is invalid, reasons:"
                           + Environment.NewLine
                           + string.Join(Environment.NewLine, errorMessages);

        logger.LogError("Config section '{ConfigurationSection}' is invalid, reasons: {@Reasons}",
            configurationSection,
            errorMessages);

        return ValidateOptionsResult.Fail(errorMessage);
    }
}