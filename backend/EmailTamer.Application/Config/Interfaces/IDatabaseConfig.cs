namespace EmailTamer.Config.Interfaces;

public interface IDatabaseConfig
{
    string ConnectionString { get; }
    int Retries { get; }
    int Timeout { get; }
}