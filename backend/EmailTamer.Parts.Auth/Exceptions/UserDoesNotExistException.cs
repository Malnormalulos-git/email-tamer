using EmailTamer.Core.Exceptions;

namespace EmailTamer.Auth.Exceptions;

public class UserDoesNotExistException(string id)
	: EmailTamerApiException($"The user with id {id} does not exist");