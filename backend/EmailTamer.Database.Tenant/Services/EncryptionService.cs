using System.Security.Cryptography;
using System.Text;
using EmailTamer.Database.Services;
using EmailTamer.Database.Tenant.Accessor;
using Microsoft.Extensions.Configuration;

namespace EmailTamer.Database.Tenant.Services;

public sealed class EncryptionService(ITenantContextAccessor tenantContextAccessor, IConfiguration configuration) : IEncryptionService
{
    private const int AesKeySizeBytes = 32; // AES-256 requires a 32-byte key
    private const int AesIvSizeBytes = 16;  // AES requires a 16-byte IV for CBC

    private async Task<byte[]> DeriveKeyFromTenantId()
    {
        var tenantId = await tenantContextAccessor.GetTenantId();
        
        var salt = configuration.GetValue<string>("Encryption:Tenant:Salt") 
            ?? throw new InvalidOperationException("Tenant encryption salt not configured.");

        using var sha256 = SHA256.Create();
        var saltedInput = Encoding.UTF8.GetBytes(tenantId + salt);
        var key = sha256.ComputeHash(saltedInput);

        Array.Resize(ref key, AesKeySizeBytes);
        return key;
    }

    public string Encrypt(string dataToEncrypt)
    {
        if (string.IsNullOrEmpty(dataToEncrypt) || string.IsNullOrWhiteSpace(dataToEncrypt))
        {
            return null;
        }

        byte[] encrypted;
        byte[] iv;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = DeriveKeyFromTenantId().GetAwaiter().GetResult();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.GenerateIV(); // random IV
            iv = aesAlg.IV;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

            using var msEncrypt = new MemoryStream();
            // Prepend the IV to the ciphertext (needed for decryption)
            msEncrypt.Write(iv, 0, iv.Length);
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(dataToEncrypt);
            }

            encrypted = msEncrypt.ToArray();
        }

        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string dataToDecrypt)
    {
        if (string.IsNullOrEmpty(dataToDecrypt) || string.IsNullOrWhiteSpace(dataToDecrypt))
        {
            return null;
        }

        var cipherWithIv = Convert.FromBase64String(dataToDecrypt);

        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromTenantId().GetAwaiter().GetResult();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Extract the IV from the first 16 bytes of the ciphertext
        var iv = new byte[AesIvSizeBytes];
        Array.Copy(cipherWithIv, 0, iv, 0, iv.Length);
        aes.IV = iv;

        var cipher = new byte[cipherWithIv.Length - iv.Length];
        Array.Copy(cipherWithIv, iv.Length, cipher, 0, cipher.Length);

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(cipher);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
}