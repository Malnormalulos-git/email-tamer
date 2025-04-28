using System.ComponentModel.DataAnnotations.Schema;
using EmailTamer.Database.Attributes;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Tenant.Entities;

[Table("EmailBoxes")]
public class EmailBox : UniqueIdEntity, IDateAuditableEntity
{
    public string? BoxName { get; set; }
    
    [EncryptProperty]
    public string? UserName { get; set; }

    [EncryptProperty]
    public string Email { get; set; } = null!;

    public bool AuthenticateByEmail { get; set; } = true;

    [EncryptProperty]
    public string Password { get; set; } = null!;

    [EncryptProperty]
    public string EmailDomainConnectionHost { get; set; } = null!;

    public int EmailDomainConnectionPort { get; set; } = 993;

    public bool UseSSl { get; set; } = true;
    
    public DateTime LastSyncAt { get; set; }
    
    public ConnectionFault? ConnectionFault { get; set; } 
    
    public List<Message> Messages { get; set; } = [];
    
    
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    
    public class Configurator : EntityConfiguration<EmailBox>
    {
        public override void Configure(EntityTypeBuilder<EmailBox> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Messages)
                .WithMany(x => x.EmailBoxes);

            builder.Property(x => x.LastSyncAt).DateTime();
        }
    }
}