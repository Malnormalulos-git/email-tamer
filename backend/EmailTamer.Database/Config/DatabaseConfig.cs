using EmailTamer.Core.Config;
using FluentValidation;

namespace EmailTamer.Database.Config;

public class DatabaseConfig : IAppConfig
{
    public string ConnectionString { get; set; } = null!;
    public int Retries { get; set; }
    public int Timeout { get; set; }
    
    public class Validator : AbstractValidator<DatabaseConfig>
    {
        public Validator()
        {
            RuleFor(x => x.ConnectionString).NotEmpty();
        }
    }
}