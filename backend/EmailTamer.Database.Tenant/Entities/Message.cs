using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Tenant.Entities;

[Table("Messages")]
public class Message : IEntity
{
    public EmailBox EmailBox { get; set; } = null!;
    
    public Guid EmailBoxId { get; set; }
    
    [Key]
    public string Id { get; set; } = null!;
    
    public string? InReplyTo { get; set; }
    
    public string? Subject { get; set; } 
    
    public string? TextBody { get; set; } 
    
    public List<string> References { get; set; } = []; // TODO: To separate table?
    
    public List<EmailAddress> From { get; set; } = []; // TODO: To separate table?
    
    public List<EmailAddress> To { get; set; } = []; // TODO: To separate table?

    public DateTime Date { get; set; }
    
    public DateTime? ResentDate { get; set; }
    
    
    public string S3FolderName { get; set; } // TODO: for attachments and body
    
    public List<string> Folders { get; set; } = []; // TODO: To separate table? 
    
    public class Configurator : EntityConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            
            builder.HasOne(x => x.EmailBox)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.EmailBoxId);
            
            builder.Property(x => x.Date).DateTime();
            
            builder.Property(x => x.ResentDate).DateTime();
            
            builder.Property(p => p.References).Json();
            
            builder.Property(p => p.Folders).Json();
            
            builder.Property(p => p.To).Json();
            
            builder.Property(p => p.From).Json();
        }
    }
}

public sealed class EmailAddress // TODO: Store avatar?
{
    public string? Name { get; set; }
    public string Address { get; set; }
    public string Domain { get; set; }
}