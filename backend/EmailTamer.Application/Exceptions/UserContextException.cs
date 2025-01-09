namespace EmailTamer.Exceptions;

public sealed class UserContextException(string message)
	: EmailTamerApiException(message);