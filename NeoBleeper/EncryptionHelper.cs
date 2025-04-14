using NeoBleeper;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private static readonly byte[] Key;
    private static readonly byte[] IV;

    static EncryptionHelper()
    {
        // Retrieve the key and IV from environment variables
        string keyBase64 = Settings1.Default.Key;
        string ivBase64 = Settings1.Default.IV;

        if (string.IsNullOrEmpty(keyBase64) || string.IsNullOrEmpty(ivBase64))
        {
            throw new ArgumentException("Key and IV must be set in environment variables.");
        }

        // Convert the base64 strings to byte arrays
        Key = Convert.FromBase64String(keyBase64);
        IV = Convert.FromBase64String(ivBase64);

        // Validate the key and IV sizes
        if (Key.Length != 16 && Key.Length != 24 && Key.Length != 32)
        {
            throw new ArgumentException("Invalid key size. Key must be 16, 24, or 32 bytes.");
        }

        if (IV.Length != 16)
        {
            throw new ArgumentException("Invalid IV size. IV must be 16 bytes.");
        }
    }

    public static string EncryptString(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string DecryptString(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
    public static void ChangeKeyAndIV()
    {
        // Generate a new key and IV
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.GenerateKey();
            aesAlg.GenerateIV();
            // Convert the key and IV to base64 strings
            string newKeyBase64 = Convert.ToBase64String(aesAlg.Key);
            string newIVBase64 = Convert.ToBase64String(aesAlg.IV);
            // Store the new key and IV in environment variables
            Settings1.Default.Key = newKeyBase64;
            Settings1.Default.IV = newIVBase64;
            Settings1.Default.Save();
        }
    }
}
