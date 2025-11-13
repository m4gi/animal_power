namespace Magi.Scripts.Utils
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEngine;

    public static class CryptoUtils
    {
        private static readonly string Password = "magi_password";
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("magi_salt");

        private static byte[] GenerateKeyFromPassword()
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(Password, Salt, 10000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32);
            }
        }

        private static byte[] GenerateRandomIV()
        {
            byte[] iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }

            return iv;
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                Debug.LogWarning("Plaintext is empty or null");
                return string.Empty;
            }

            try
            {
                byte[] key = GenerateKeyFromPassword();
                byte[] iv = GenerateRandomIV();

                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }

                        return Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Encryption failed: {ex.Message}");
                return string.Empty;
            }
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                Debug.LogWarning("Encrypted text is empty or null");
                return string.Empty;
            }

            try
            {
                string[] parts = encryptedText.Split(':');
                if (parts.Length != 2)
                {
                    Debug.LogError("Invalid encrypted data format");
                    return string.Empty;
                }

                byte[] iv = Convert.FromBase64String(parts[0]);
                byte[] cipherBytes = Convert.FromBase64String(parts[1]);
                byte[] key = GenerateKeyFromPassword();

                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Decryption failed: {ex.Message}");
                return string.Empty;
            }
        }
    }
}