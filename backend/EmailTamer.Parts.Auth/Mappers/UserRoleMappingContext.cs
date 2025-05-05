using EmailTamer.Core.Mappers;
using EmailTamer.Infrastructure.Auth;

namespace EmailTamer.Auth.Mappers;

public sealed record UserRoleMappingContext(UserRole Role) : IMappingContext
{
    public IDictionary<string, object> EnrichContext(IDictionary<string, object> context)
    {
        context[nameof(Role)] = Role;
        return context;
    }

    public static UserRoleMappingContext FromContext(IDictionary<string, object> context)
        => new((UserRole)context[nameof(Role)]);
}