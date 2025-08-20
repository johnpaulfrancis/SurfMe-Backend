using Microsoft.Extensions.Options;
using SurfMe.Models;
using System.Security.Cryptography;
using System.Text;

namespace SurfMe.Service
{
    public class EncryptDecryptService
    {
        private readonly string _secretKey;

        public EncryptDecryptService(IOptions<EncryptDecryptServiceModel> options)
        {
            _secretKey = options.Value.EncryptionKey;
        }

        public string Encrypt(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes(_secretKey);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            return Base64UrlEncode(Convert.ToBase64String(result));
        }

        public string Decrypt(string cipherText)
        {
            if(string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }
            cipherText = Base64UrlDecode(cipherText);
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] key = Encoding.UTF8.GetBytes(_secretKey);

            using var aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Base64 Url Encode
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        /// <summary>
        /// Base64 Url Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static string Base64UrlDecode(string encoded)
        {
            encoded = encoded.Replace('-', '+').Replace('_', '/');
            switch (encoded.Length % 4)
            {
                case 2: encoded += "=="; break;
                case 3: encoded += "="; break;
            }
            var bytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
