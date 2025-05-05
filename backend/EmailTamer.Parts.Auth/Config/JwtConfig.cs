using EmailTamer.Core.Config;
using FluentValidation;

namespace EmailTamer.Auth.Config;

public class JwtConfig : IAppConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Authority { get; set; } = null!;
    public string Key { get; set; } = null!;

    public class Validator : AbstractValidator<JwtConfig>
    {
        public Validator()
        {
            RuleFor(x => x.Issuer).NotEmpty();
            RuleFor(x => x.Audience).NotEmpty();
            RuleFor(x => x.Authority).NotEmpty();
            RuleFor(x => x.Key).NotEmpty();
        }
    }
}