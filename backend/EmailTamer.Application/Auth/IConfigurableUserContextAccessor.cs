namespace EmailTamer.Auth;

internal interface IConfigurableUserContextAccessor : IUserContextAccessor
{
	void Configure(string userId);
}