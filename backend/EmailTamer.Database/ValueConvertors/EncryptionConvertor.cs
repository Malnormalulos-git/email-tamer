using EmailTamer.Database.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmailTamer.Database.ValueConvertors;

public class EncryptionConvertor(IEncryptionService? encryptionService, ConverterMappingHints mappingHints = null)
    : ValueConverter<string, string>(
        x => encryptionService.Encrypt(x),
        x => encryptionService.Decrypt(x),
        mappingHints)
{
}