using EmailTamer.Database.Attributes;
using EmailTamer.Database.Services;
using EmailTamer.Database.ValueConvertors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Database.Extensions;

public static class ModelPropertyEncrypterExtension
{
    public static void UseEncryption(this ModelBuilder modelBuilder, IEncryptionService encryptionService, ILogger logger = null)
    {
        var converter = new EncryptionConvertor(encryptionService);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) && !IsDiscriminator(property))
                {
                    var attributes = property.PropertyInfo?.GetCustomAttributes(typeof(EncryptPropertyAttribute), false);
                    if (attributes != null && attributes.Any())
                    {
                        logger?.LogInformation("Applying EncryptionConvertor to property {PropertyName} in entity {EntityName}",
                            property.Name, entityType.Name);
                        property.SetValueConverter(converter);
                    }
                }
            }
        }
    }

    // A helper function to ignore EF Core Discriminator
    private static bool IsDiscriminator(IMutableProperty property)
    {
        return property.Name == "Discriminator" || property.PropertyInfo == null;
    }
}