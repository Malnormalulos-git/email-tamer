using EmailTamer.Core.Config;

namespace EmailTamer.Database.Config;

public class DatabaseConfig : IAppConfig
{
    public string ConnectionString { get; set; } = null!;
    public int Retries { get; set; }
    public int Timeout { get; set; }
}