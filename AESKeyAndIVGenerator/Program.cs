using System.Security.Cryptography;

// 32 byte uzunluğunda anahtar oluştur
byte[] key = new byte[32];
using (var rng = new RNGCryptoServiceProvider())
{
    rng.GetBytes(key);
}
string base64Key = Convert.ToBase64String(key);
Console.WriteLine("Key: " + base64Key);

// 16 byte uzunluğunda IV oluştur
byte[] iv = new byte[16];
using (var rng = new RNGCryptoServiceProvider())
{
    rng.GetBytes(iv);
}
string base64IV = Convert.ToBase64String(iv);
Console.WriteLine("IV: " + base64IV);