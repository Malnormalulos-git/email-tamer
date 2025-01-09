using MediatR;

namespace EmailTamer.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetUpMediatR(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}