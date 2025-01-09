using EmailTamer.Config.Interfaces;

namespace EmailTamer.Config;

public class DatabaseConfig : IDatabaseConfig
{
    public string ConnectionString { get; set; } = null!;
    public int Retries { get; set; }
    public int Timeout { get; set; }
}