using System.ComponentModel.DataAnnotations.Schema;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Entities.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailTamer.Database.Tenant.Entities;

[Table("Folders")]
public class Folder : UniqueIdEntity
{
    public string Name { get; set; } = null!;

    public List<Message> Messages { get; set; } = [];

    public class Configurator : EntityConfiguration<Folder>
    {
        public override void Configure(EntityTypeBuilder<Folder> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Messages)
                .WithMany(x => x.Folders);
        }
    }
}