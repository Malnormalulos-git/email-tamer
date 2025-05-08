using System.Security.Cryptography;
using System.Text;
using EmailTamer.Database.Services;
using EmailTamer.Database.Tenant.Accessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Database.Tenant.Services;

public sealed class EncryptionService(
    ITenantContextAccessor tenantContextAccessor,
    IConfiguration configuration,
    ILogger<EncryptionService> logger)
    : IEncryptionService
{
    private const int AesKeySizeBytes = 32; // AES-256 requires a 32-byte key
    private const int AesIvSizeBytes = 16;  // AES requires a 16-byte IV for CBC

    private byte[] DeriveKeyFromTenantId()
    {
        try
        {
            var tenantId = tenantContextAccessor.GetTenantId().GetAwaiter().GetResult();

            var salt = configuration.GetValue<string>("Encryption:Tenant:Salt")
                ?? throw new InvalidOperationException("Tenant encryption salt not configured.");

            if (string.IsNullOrEmpty(salt))
            {
                logger.LogError("Encryption salt is empty or not configured.");
                throw new InvalidOperationException("Encryption salt cannot be empty.");
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(
                Encoding.UTF8.GetBytes(tenantId),
                Encoding.UTF8.GetBytes(salt),
                100000,
                HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(AesKeySizeBytes);

            return key;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to derive encryption key for tenant.");
            throw;
        }
    }

    public string Encrypt(string dataToEncrypt)
    {
        if (string.IsNullOrEmpty(dataToEncrypt) || string.IsNullOrWhiteSpace(dataToEncrypt))
        {
            logger.LogWarning("Attempted to encrypt empty or whitespace data.");
            return null;
        }

        try
        {
            byte[] encrypted;
            byte[] iv;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = DeriveKeyFromTenantId();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.GenerateIV();
                iv = aesAlg.IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

                using var msEncrypt = new MemoryStream();
                msEncrypt.Write(iv, 0, iv.Length);
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(dataToEncrypt);
                }

                encrypted = msEncrypt.ToArray();
            }

            var result = Convert.ToBase64String(encrypted);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Encryption failed for provided data.");
            throw new CryptographicException("Failed to encrypt data.", ex);
        }
    }

    public string Decrypt(string dataToDecrypt)
    {
        if (string.IsNullOrEmpty(dataToDecrypt) || string.IsNullOrWhiteSpace(dataToDecrypt))
        {
            logger.LogWarning("Attempted to decrypt empty or whitespace data.");
            return null;
        }

        try
        {
            if (dataToDecrypt.Length < Convert.ToBase64String(new byte[AesIvSizeBytes]).Length)
            {
                logger.LogError("Encrypted data is too short to contain a valid IV.");
                throw new ArgumentException("Encrypted data is too short to contain a valid IV.");
            }

            var cipherWithIv = Convert.FromBase64String(dataToDecrypt);

            if (cipherWithIv.Length < AesIvSizeBytes)
            {
                logger.LogError("Encrypted data does not contain enough bytes for IV.");
                throw new ArgumentException("Encrypted data does not contain enough bytes for IV.");
            }

            using var aes = Aes.Create();
            aes.Key = DeriveKeyFromTenantId();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[AesIvSizeBytes];
            Array.Copy(cipherWithIv, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var cipher = new byte[cipherWithIv.Length - iv.Length];
            Array.Copy(cipherWithIv, iv.Length, cipher, 0, cipher.Length);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(cipher);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            var result = streamReader.ReadToEnd();
            return result;
        }
        catch (FormatException ex)
        {
            logger.LogError(ex, "Invalid Base64 string provided for decryption.");
            throw new ArgumentException("Invalid Base64 string provided for decryption.", ex);
        }
        catch (CryptographicException ex)
        {
            logger.LogError(ex, "Decryption failed due to cryptographic error.");
            throw new CryptographicException("Failed to decrypt data.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during decryption.");
            throw new CryptographicException("Unexpected error during decryption.", ex);
        }
    }
}