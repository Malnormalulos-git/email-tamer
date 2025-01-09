namespace EmailTamer.Config.Interfaces;

public interface IApplicationConfig
{
    IDatabaseConfig Database { get; }
    IJwtConfig Jwt { get; }
}