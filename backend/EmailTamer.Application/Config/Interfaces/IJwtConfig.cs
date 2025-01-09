namespace EmailTamer.Config.Interfaces;

public interface IJwtConfig
{
    string Issuer { get; }
    string Audience { get; }
    string Authority { get; }
    string Key { get; }
}