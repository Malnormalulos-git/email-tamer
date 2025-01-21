using EmailTamer.Core.Config;
using FluentValidation;

namespace EmailTamer.Database.Tenant.Config;

public class TenantsDatabaseConfig : IAppConfig
{
    public string DefaultConnectionString { get; set; } = null!;
    public int Retries { get; set; }
    public int Timeout { get; set; }
    
    public class Validator : AbstractValidator<TenantsDatabaseConfig>
    {
        public Validator()
        {
            RuleFor(x => x.DefaultConnectionString).NotEmpty();
        }
    }
}