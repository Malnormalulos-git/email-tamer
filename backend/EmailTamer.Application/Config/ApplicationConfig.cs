using EmailTamer.Config.Interfaces;

namespace EmailTamer.Config;

public class ApplicationConfig : IApplicationConfig
{
    public IDatabaseConfig Database { get; } = new DatabaseConfig();
    public IJwtConfig Jwt { get; } = new JwtConfig();
}