using EmailTamer.Core.Mappers;
using EmailTamer.Infrastructure.Auth;

namespace EmailTamer.Auth.Mappers;

public sealed record AuthUserMappingContext(AuthUser User) : IMappingContext
{
	public IDictionary<string, object> EnrichContext(IDictionary<string, object> context)
	{
		context[nameof(User)] = User;
		return context;
	}

	public static AuthUserMappingContext FromContext(IDictionary<string, object> context)
		=> new((AuthUser)context[nameof(User)]);
}