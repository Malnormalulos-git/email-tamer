using EmailTamer.Exceptions;

namespace EmailTamer.Auth;

internal sealed class UserContextAccessor : IConfigurableUserContextAccessor
{
	private bool _configured;

	public void Configure(string userId)
	{
		if (_configured)
		{
			throw new UserContextException("User context has been configured already");
		}

		Id = userId;

		_configured = true;
	}

	public string Id { get; set; } = null!;
}