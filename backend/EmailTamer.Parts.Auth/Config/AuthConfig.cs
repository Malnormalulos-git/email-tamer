using EmailTamer.Core.Config;
using FluentValidation;
using JetBrains.Annotations;

namespace EmailTamer.Auth.Config;

public sealed class AuthConfig : IAppConfig
{
	public string[] AllowAnonymous { get; set; } = Array.Empty<string>();

	[UsedImplicitly]
	public class Validator : AbstractValidator<AuthConfig>
	{
		public Validator() => RuleForEach(x => x.AllowAnonymous).NotNull().NotEmpty();
	}
}