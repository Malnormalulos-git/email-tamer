using EmailTamer.Core.Exceptions;

namespace EmailTamer.Auth.Exceptions;

public sealed class UserContextException(string message)
	: EmailTamerApiException(message);