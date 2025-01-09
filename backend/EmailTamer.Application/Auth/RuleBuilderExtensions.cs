using FluentValidation;

namespace EmailTamer.Auth;

public static class RuleBuilderExtensions
{
	public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilderOptions<T, string> builder)
		=> builder.Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")
			.WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");

	public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilderOptions<T, string> builder)
		=> builder.Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
			.WithMessage("Email must match the email pattern");
}