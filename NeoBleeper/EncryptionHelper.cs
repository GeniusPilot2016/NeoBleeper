// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using NeoBleeper;
using System.Security.Cryptography;

public class EncryptionHelper
{
    // Changed from readonly to allow updating
    private static byte[] _key;
    private static byte[] _iv;

    /// <summary>
    /// Gets the cryptographic key used for encryption and decryption operations.
    /// </summary>
    /// <remarks>The key is retrieved from application settings and is expected to be a Base64-encoded string.
    /// Accessing this property will throw an exception if the key is not set in the configuration.</remarks>
    private static byte[] Key
    {
        get
        {
            // Only convert from Base64 if needed
            if (_key == null)
            {
                string keyBase64 = Settings1.Default.Key;
                if (string.IsNullOrEmpty(keyBase64))
                {
                    throw new ArgumentException("Key must be set in settings.");
                }
                _key = Convert.FromBase64String(keyBase64);
            }
            return _key;
        }
    }

    /// <summary>
    /// Gets the initialization vector (IV) used for cryptographic operations.
    /// </summary>
    /// <remarks>The IV is retrieved from application settings as a Base64-encoded string and converted to a
    /// byte array on first access. The value must be set in the application settings before use.</remarks>
    private static byte[] IV
    {
        get
        {
            // Only convert from Base64 if needed
            if (_iv == null)
            {
                string ivBase64 = Settings1.Default.IV;
                if (string.IsNullOrEmpty(ivBase64))
                {
                    throw new ArgumentException("IV must be set in settings.");
                }
                _iv = Convert.FromBase64String(ivBase64);
            }
            return _iv;
        }
    }

    /// <summary>
    /// Encrypts the specified plain text string using the configured AES key and initialization vector (IV), and
    /// returns the result as a Base64-encoded string.
    /// </summary>
    /// <remarks>The method uses AES encryption with the key and IV provided by the static fields. Ensure that
    /// the key and IV are securely generated and managed. The caller is responsible for providing valid key and IV
    /// values before calling this method.</remarks>
    /// <param name="plainText">The plain text string to encrypt. If null or empty, the method returns an empty string.</param>
    /// <returns>A Base64-encoded string containing the encrypted representation of the input plain text. Returns an empty string
    /// if the input is null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown if the configured AES key is not 16, 24, or 32 bytes in length, or if the IV is not 16 bytes in length.</exception>
    public static string EncryptString(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            // Validate key and IV sizes
            if (Key.Length != 16 && Key.Length != 24 && Key.Length != 32)
            {
                throw new ArgumentException("Invalid key size. Key must be 16, 24, or 32 bytes.");
            }

            if (IV.Length != 16)
            {
                throw new ArgumentException("Invalid IV size. IV must be 16 bytes.");
            }

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
        catch (Exception ex)
        {
            Logger.Log("Encryption error: " + ex.Message, Logger.LogTypes.Error);
            throw;
        }
    }

    /// <summary>
    /// Decrypts the specified Base64-encoded string using the configured AES key and initialization vector (IV).
    /// </summary>
    /// <remarks>The method expects the AES key and IV to be set and valid before calling. If decryption fails
    /// due to an invalid input or cryptographic error, the exception is logged and rethrown.</remarks>
    /// <param name="cipherText">The encrypted string to decrypt, encoded in Base64. Cannot be null or empty.</param>
    /// <returns>The decrypted plain text string. Returns an empty string if the input is null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown if the configured AES key is not 16, 24, or 32 bytes in length, or if the IV is not 16 bytes.</exception>
    public static string DecryptString(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            // Validate key and IV sizes
            if (Key.Length != 16 && Key.Length != 24 && Key.Length != 32)
            {
                throw new ArgumentException("Invalid key size. Key must be 16, 24, or 32 bytes.");
            }

            if (IV.Length != 16)
            {
                throw new ArgumentException("Invalid IV size. IV must be 16 bytes.");
            }

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
        catch (Exception ex)
        {
            Logger.Log("Decryption error: " + ex.Message, Logger.LogTypes.Error);
            throw;
        }
    }

    /// <summary>
    /// Generates and stores a new encryption key and initialization vector (IV) for AES encryption in the application
    /// settings.
    /// </summary>
    /// <remarks>Call this method to rotate the application's encryption key and IV. After calling this
    /// method, any data encrypted with the previous key and IV will no longer be decryptable unless the old values are
    /// preserved elsewhere. This operation updates the stored key and IV and resets any cached values to ensure the new
    /// credentials are used in subsequent cryptographic operations.</remarks>
    public static void ChangeKeyAndIV()
    {
        try
        {
            // Create new AES instance and generate a new key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                // Convert the new key and IV to base64 strings
                string newKeyBase64 = Convert.ToBase64String(aesAlg.Key);
                string newIVBase64 = Convert.ToBase64String(aesAlg.IV);

                // Store the new key and IV in settings
                Settings1.Default.Key = newKeyBase64;
                Settings1.Default.IV = newIVBase64;
                Settings1.Default.Save();

                // Reset the static fields to force them to be reloaded
                _key = null;
                _iv = null;

                Logger.Log("Generated new encryption key and IV", Logger.LogTypes.Info);
            }
        }
        catch (Exception ex)
        {
            Logger.Log("Error changing encryption key and IV: " + ex.Message, Logger.LogTypes.Error);
            throw;
        }
    }
}
