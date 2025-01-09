using EmailTamer.Exceptions;

namespace EmailTamer.Exceptions;

public sealed class EmailTamerValidationException : EmailTamerApiException
{
	private const string PrimaryMessage = "The server couldn`t make sense of your request";

	public EmailTamerValidationException(string validationError) : base($"{PrimaryMessage}: {validationError}")
	{
	}

	public EmailTamerValidationException(IEnumerable<string> errors) : base(
		$"{PrimaryMessage}: {errors.Aggregate((prev, next) => $"{prev}, {next}")}")
	{
	}

	public override int StatusCode { get; set; } = 400;
}