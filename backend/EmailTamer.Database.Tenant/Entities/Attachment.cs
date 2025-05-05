using System.ComponentModel.DataAnnotations.Schema;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Tenant.Entities;

[Table("Attachments")]
public sealed class Attachment : UniqueIdEntity
{
    public string FileName { get; set; } = null!;

    public Message Message { get; set; } = null!;

    public string MessageId { get; set; } = null!;

    public class Configurator : EntityConfiguration<Attachment>
    {
        public override void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Message)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.MessageId);
        }
    }
}