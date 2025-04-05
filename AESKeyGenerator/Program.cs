using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private static readonly byte[] Key = Convert.FromBase64String(Environment.GetEnvironmentVariable("Key"));
    private static readonly byte[] IV = Convert.FromBase64String(Environment.GetEnvironmentVariable("IV"));

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
}

class Program
{
    static void Main()
    {
        // Generate a new key and IV
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.GenerateKey();
            aesAlg.GenerateIV();

            string keyBase64 = Convert.ToBase64String(aesAlg.Key);
            string ivBase64 = Convert.ToBase64String(aesAlg.IV);

            // Display the generated key and IV
            Console.WriteLine("Generated Key: " + keyBase64);
            Console.WriteLine("Generated IV: " + ivBase64);

            // Optionally, store these values in environment variables
            Environment.SetEnvironmentVariable("Key", keyBase64);
            Environment.SetEnvironmentVariable("IV", ivBase64);
        }
    }
}
