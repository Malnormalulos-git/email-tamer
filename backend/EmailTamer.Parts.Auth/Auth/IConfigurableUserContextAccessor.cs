using EmailTamer.Infrastructure.Auth;

namespace EmailTamer.Auth;

public interface IConfigurableUserContextAccessor : IUserContextAccessor
{
	void Configure(string userId);
}