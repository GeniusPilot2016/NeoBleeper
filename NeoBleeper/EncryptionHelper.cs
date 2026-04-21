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
using NeoBleeper.Properties;
using System.Buffers.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Windows.ApplicationModel.Activation;
using static NeoBleeper.Logger;

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
    [Obsolete("This function is replaced with DPAPI", error: false)]
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
    [Obsolete("This function is replaced with DPAPI", error: false)]
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
    [Obsolete("This function is replaced with DPAPI", error: true)]
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
        finally
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(plainText)); // Clear the input plain text from memory
            }
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
    [Obsolete("This function is replaced with DPAPI", error:false)]
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
        finally
        {
            if(!string.IsNullOrEmpty(cipherText))
            {
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(cipherText)); // Clear the input cipher text from memory
            }
        }
    }

    /// <summary>
    /// Encrypts the specified plain text using the current user's data protection scope and returns the result as a
    /// Base64-encoded string.
    /// </summary>
    /// <remarks>The encrypted data can only be decrypted by the same user account on the same machine. Use
    /// this method to securely store or transmit sensitive information in a form that is tied to the current
    /// user.</remarks>
    /// <param name="plainText">The plain text string to encrypt. Cannot be null.</param>
    /// <returns>A Base64-encoded string representing the encrypted form of the input plain text.</returns>
    public string GenerateEncryptedStringBase64(string plainText)
    {
        byte[] bytes = null;
        byte[] encryptedString = null;
        string outputBase64 = null;
        try
        {
            // Convert the plain text string to a byte array using Unicode encoding
            bytes = UnicodeEncoding.Unicode.GetBytes(plainText);
            // Encrypt the byte array using DPAPI with the current user scope
            encryptedString = ProtectedData.Protect(bytes, DataProtectionScope.CurrentUser);
            // Convert the encrypted byte array to a Base64 string for easier storage and transmission
            outputBase64 = Convert.ToBase64String(encryptedString);
            return outputBase64;
        }
        finally
        {
            // Clear the plain text byte array from memory
            if (!string.IsNullOrEmpty(plainText)){
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(plainText));
            }
            if(bytes != null){
                CryptographicOperations.ZeroMemory(bytes);
            }
            if (encryptedString != null){
                CryptographicOperations.ZeroMemory(encryptedString);
            }
            if (!string.IsNullOrEmpty(outputBase64))
            {
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(outputBase64));
            }
        }
    }

    public bool TryDecryptStringBase64(string plainText)
    {
        byte[] encryptedBytes = null;
        byte[] decryptedBytes = null;
        string decryptedString = null;
        try
        {
            // Convert the Base64-encoded string back to a byte array
            encryptedBytes = Convert.FromBase64String(plainText);
            // Decrypt the byte array using DPAPI with the current user scope
            decryptedBytes = ProtectedData.Unprotect(encryptedBytes, DataProtectionScope.CurrentUser);
            // Convert the decrypted byte array back to a string using Unicode encoding
            decryptedString = UnicodeEncoding.Unicode.GetString(decryptedBytes);
            return true;

        }
        catch (Exception ex)
        {
            Logger.Log("TryDecryptStringBase64 error: " + ex.Message, Logger.LogTypes.Error);
            return false;
        }
        finally
        {
            if (encryptedBytes != null){
                CryptographicOperations.ZeroMemory(encryptedBytes); // Clear the encrypted byte array from memory
            }
            if (decryptedBytes != null){
                CryptographicOperations.ZeroMemory(decryptedBytes);
            }
            if (!string.IsNullOrEmpty(decryptedString)){
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(decryptedString)); // Clear the decrypted string from memory
            }
            if(!string.IsNullOrEmpty(plainText)){
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(plainText)); // Clear the input string from memory
            }
        }
    }

    /// <summary>
    /// Attempts to decrypt the legacy API key using the current application settings.
    /// </summary>
    /// <remarks>This method does not throw an exception if decryption fails. Instead, it returns false to
    /// indicate failure.</remarks>
    /// <returns>true if the legacy API key is successfully decrypted; otherwise, false.</returns>
    private bool TryDecryptLegacyAPIKey()
    {
        string apiKey = null;
        try
        {
            apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
            return true;
        }
        catch (CryptographicException ex)
        {
            return false;
        }
        finally
        {
            if (!string.IsNullOrEmpty(apiKey)) {
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(apiKey)); // Clear the decrypted API key from memory
            }
        }
    }

    /// <summary>
    /// Decrypts a Base64-encoded string that was encrypted using Windows Data Protection API (DPAPI) for the current
    /// user.
    /// </summary>
    /// <remarks>This method uses the current user's data protection scope to decrypt the input. If the input
    /// was not encrypted with the same user context or is not a valid Base64-encoded DPAPI payload, an exception will
    /// be thrown.</remarks>
    /// <param name="encryptedDataBase64">The Base64-encoded string representing data encrypted with DPAPI. Cannot be null or empty.</param>
    /// <returns>The decrypted string if decryption is successful; otherwise, an empty string if the input is null or empty.</returns>
    public string DecryptBase64EncryptedData(string encryptedDataBase64)
    {
        if (string.IsNullOrEmpty(encryptedDataBase64))
            return string.Empty;

        byte[] protectedBytes = null;
        byte[] plainBytes = null;
        try
        {
            // Convert Base64 string to byte array
            protectedBytes = Convert.FromBase64String(encryptedDataBase64);

            // Decrypt the byte array using DPAPI
            plainBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);

            // Convert the decrypted byte array back to a string
            return UnicodeEncoding.Unicode.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            Logger.Log("DecryptBase64EncryptedData error: " + ex.Message, Logger.LogTypes.Error);
            throw;
        }
        finally
        {
            // Clear sensitive data from memory
            if (protectedBytes != null)
                CryptographicOperations.ZeroMemory(protectedBytes);
            if (plainBytes != null)
                CryptographicOperations.ZeroMemory(plainBytes);
            if(!string.IsNullOrEmpty(encryptedDataBase64))
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(encryptedDataBase64));
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
    [Obsolete("This function is replaced with DPAPI", error: true)]
    public static void ChangeKeyAndIV()
    {
        string newKeyBase64 = null;
        string newIVBase64 = null;
        try
        {
            // Create new AES instance and generate a new key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                // Convert the new key and IV to base64 strings
                newKeyBase64 = Convert.ToBase64String(aesAlg.Key);
                newIVBase64 = Convert.ToBase64String(aesAlg.IV);

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
        finally
        {
            if (!string.IsNullOrEmpty(newKeyBase64))
            {
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(newKeyBase64)); // Clear the new key from memory
            }
            if (!string.IsNullOrEmpty(newIVBase64))
            {
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(newIVBase64)); // Clear the new IV from memory
            }
        }
    }

    /// <summary>
    /// Migrates a legacy API key stored in application settings to the new encrypted format, if present.
    /// </summary>
    /// <remarks>This method checks for the existence of a legacy API key and related cryptographic values in
    /// the application settings. If found and successfully decrypted, the key is re-encrypted using the current
    /// encryption scheme and stored in the updated format. The legacy key and associated values are then cleared from
    /// the settings to prevent reuse. This operation is intended to be called during application upgrade scenarios
    /// where API key storage has changed.</remarks>
    public void MigrateSavedLegacyAPIKey()
    {
        if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey) || 
            !string.IsNullOrEmpty(Settings1.Default.Key) ||
            !string.IsNullOrEmpty(Settings1.Default.IV))
        {
            if(TryDecryptLegacyAPIKey())
            {
                Debug.WriteLine("Successfully decrypted legacy API key. Proceeding with migration.");
                string legacyAPIKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                Settings1.Default.EncryptedGeminiAPIKeyBase64 = GenerateEncryptedStringBase64(legacyAPIKey);
                CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(legacyAPIKey)); // Clear the decrypted API key from memory
            }
            else
            {
                Debug.WriteLine("Failed to decrypt legacy API key. Migration aborted.");
            }
            Settings1.Default.geminiAPIKey = string.Empty; // Clear the legacy key
            Settings1.Default.Key = string.Empty; // Clear the legacy key
            Settings1.Default.IV = string.Empty; // Clear the legacy IV
        }
    }
}
