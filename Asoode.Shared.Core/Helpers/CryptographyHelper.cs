using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Asoode.Shared.Core.Helpers;

public static class CryptographyHelper
{
    private const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
    private const int DerivationIterations = 1000;

    public static string ComputeSHA256(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashedBytes = sha256.ComputeHash(inputBytes);

            var builder = new StringBuilder();
            for (var i = 0; i < hashedBytes.Length; i++) builder.Append(hashedBytes[i].ToString("x2"));

            return builder.ToString();
        }
    }

    public static string GenerateSalt(int length = 32)
    {
        var salt = new byte[length];

        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(salt);
        }

        return Convert.ToBase64String(salt);
    }

    public static string Hash(string input, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var bytes = KeyDerivation.Pbkdf2(
            input, saltBytes, KeyDerivationPrf.HMACSHA512, 10000, 16);

        return Convert.ToBase64String(bytes);
    }

    public static bool Verify(string input, string hash, string salt)
    {
        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            var bytes = KeyDerivation.Pbkdf2(input, saltBytes, KeyDerivationPrf.HMACSHA512, 10000, 16);
            var encoded = Convert.ToBase64String(bytes);
            return hash.Equals(encoded);
        }
        catch
        {
            return false;
        }
    }

    public static string DecryptRijndael(string cipherText, string key, int blockSize = 128)
    {
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(blockSize / 8).ToArray();
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(blockSize / 8).Take(blockSize / 8).ToArray();
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(blockSize / 8 * 2)
            .Take(cipherTextBytesWithSaltAndIv.Length - blockSize / 8 * 2).ToArray();

        using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(blockSize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = blockSize;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
    }

    public static string DecryptSHA512(string encryptedText, string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key must have valid value.", nameof(key));
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentException("The encrypted text must have valid value.", nameof(encryptedText));

        var combined = Convert.FromBase64String(encryptedText);
        var buffer = new byte[combined.Length];
        var hash = new SHA512CryptoServiceProvider();
        var aesKey = new byte[24];
        Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

        using (var aes = Aes.Create())
        {
            if (aes == null)
                throw new ArgumentException("Parameter must not be null.", nameof(aes));

            aes.Key = aesKey;

            var iv = new byte[aes.IV.Length];
            var ciphertext = new byte[buffer.Length - iv.Length];

            Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
            Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var resultStream = new MemoryStream())
            {
                using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                using (var plainStream = new MemoryStream(ciphertext))
                {
                    plainStream.CopyTo(aesStream);
                }

                return Encoding.UTF8.GetString(resultStream.ToArray());
            }
        }
    }

    public static string EncryptRijndael(string plainText, string key, int blockSize = 128)
    {
        var saltStringBytes = Generate256BitsOfRandomEntropy(blockSize);
        var ivStringBytes = Generate256BitsOfRandomEntropy(blockSize);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(blockSize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = blockSize;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public static string GeneratePassword(int length = 10)
    {
        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = validChars[RandomNumberGenerator.GetInt32(0, validChars.Length)];
        return new string(chars);
    }

    public static string EncryptSHA512(string text, string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key must have valid value.", nameof(key));
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("The text must have valid value.", nameof(text));

        var buffer = Encoding.UTF8.GetBytes(text);
        var hash = new SHA512CryptoServiceProvider();
        var aesKey = new byte[24];
        Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

        using (var aes = Aes.Create())
        {
            if (aes == null)
                throw new ArgumentException("Parameter must not be null.", nameof(aes));

            aes.Key = aesKey;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var resultStream = new MemoryStream())
            {
                using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                using (var plainStream = new MemoryStream(buffer))
                {
                    plainStream.CopyTo(aesStream);
                }

                var result = resultStream.ToArray();
                var combined = new byte[aes.IV.Length + result.Length];
                Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

                return Convert.ToBase64String(combined);
            }
        }
    }

    private static byte[] Generate256BitsOfRandomEntropy(int blockSize = 256)
    {
        var randomBytes = new byte[blockSize / 8];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(randomBytes);
        }

        return randomBytes;
    }
}