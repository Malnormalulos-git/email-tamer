namespace EmailTamer.Database.Services;

public interface IEncryptionService
{
    string Encrypt(string value);
    string Decrypt(string value);
}