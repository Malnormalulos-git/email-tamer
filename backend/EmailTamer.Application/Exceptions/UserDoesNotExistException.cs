namespace EmailTamer.Exceptions;

public class UserDoesNotExistException(string id)
	: EmailTamerApiException($"The user with id {id} does not exist");