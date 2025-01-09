using EmailTamer.Config.Interfaces;

namespace EmailTamer.Config;

public class JwtConfig : IJwtConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Authority { get; set; } = null!;
    public string Key { get; set; } = null!;
}