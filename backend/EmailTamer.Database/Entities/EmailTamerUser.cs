using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Entities;

public class EmailTamerUser : IdentityUser, IEntity
{
    public Tenant? Tenant { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public class Configurator : EntityConfiguration<EmailTamerUser>
    {
        public override void Configure(EntityTypeBuilder<EmailTamerUser> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Tenant)
                .WithOne(x => x.Owner)
                .HasForeignKey<Tenant>(x => x.OwnerId);
        }
    }
}