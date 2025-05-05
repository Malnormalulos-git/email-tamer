using EmailTamer.Infrastructure.Auth;

namespace EmailTamer.Auth.Auth;

public interface IConfigurableUserContextAccessor : IUserContextAccessor
{
    void Configure(string userId);
}