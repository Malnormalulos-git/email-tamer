using EmailTamer.Parts.EmailBox.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.EmailBox;

public static class ServiceCollectionExtensions
{
    public static IMvcCoreBuilder AddEmailBoxesPart(this IMvcCoreBuilder builder)
    {
        builder.AddEmailTamerPart<EmailBoxesController>();
        return builder;
    }
}