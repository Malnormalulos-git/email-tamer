using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using EmailTamer.Database.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Tenant.Entities;

[Table("Messages")]
public sealed class Message : IEntity
{
    public List<EmailBox> EmailBoxes { get; set; } = [];
    
    [Key]
    public string Id { get; set; } = null!;
    
    public string? ThreadId { get; set; }
    
    public string? InReplyTo { get; set; }
    
    public string? Subject { get; set; } 
    
    public string? TextBody { get; set; } 
    
    public List<Attachment> Attachments { get; set; } = [];
    
    public List<string> References { get; set; } = []; // TODO: To separate table?
    
    public List<EmailAddress> From { get; set; } = []; // TODO: To separate table?
    
    public List<EmailAddress> To { get; set; } = []; // TODO: To separate table?

    public DateTime Date { get; set; }
    
    public DateTime? ResentDate { get; set; }
    
    public List<Folder> Folders { get; set; } = [];  
    
    public bool HasHtmlBody { get; set; }
    
    public class Configurator : EntityConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            
            
            builder.HasMany(x => x.EmailBoxes)
                .WithMany(x => x.Messages);
            
            builder.HasMany(x => x.Folders)
                .WithMany(x => x.Messages);
            
            builder.HasMany(x => x.Attachments)
                .WithOne(x => x.Message)
                .HasForeignKey(x => x.MessageId);

            builder.HasIndex(x => x.ThreadId);
            
            
            builder.Property(x => x.Date).DateTime();
            
            builder.Property(x => x.ResentDate).DateTime();
            

            builder.Property(x => x.References).Json();
            
            builder.Property(x => x.To).Json();
            
            builder.Property(x => x.From).Json();
        }
    }
}

public sealed class EmailAddress 
{
    public string? Name { get; set; }
    public string Address { get; set; }
    public string Domain { get; set; }
}