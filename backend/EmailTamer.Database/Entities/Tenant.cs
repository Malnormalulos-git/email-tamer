using System.ComponentModel.DataAnnotations.Schema;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Entities;

[Table("Tenants")]
public class Tenant : UniqueIdEntity
{
    public EmailTamerUser Owner { get; set; } = null!;
    
    public Guid OwnerId { get; set; }
    
    public class Configurator : EntityConfiguration<Tenant>
    {
        public override void Configure(EntityTypeBuilder<Tenant> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Owner)
                .WithOne(x => x.Tenant)
                .HasForeignKey<EmailTamerUser>(x => x.TenantId);
        }
    }
}