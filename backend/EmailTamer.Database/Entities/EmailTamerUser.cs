using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Entities;

public class EmailTamerUser : IdentityUser, IEntity
{
    public Guid TenantId { get; set; }

    public class Configurator : EntityConfiguration<EmailTamerUser>;
}