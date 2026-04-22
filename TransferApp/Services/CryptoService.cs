using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptoService
{
    private readonly string _secretKey;

    public CryptoService(string secretKey)
    {
        _secretKey = secretKey;
    }

    private byte[] GetKey(string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            _secretKey,
            Encoding.UTF8.GetBytes(salt),
            100000,
            HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(32);
    }

    public string Encrypt(string plainText)
    {
        var iv = RandomNumberGenerator.GetBytes(16);
        var salt = Guid.NewGuid().ToString();
        var key = GetKey(salt);
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainText);
        sw.Close();
        var cipherBytes = ms.ToArray();
        var result = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(salt)
            .Concat(iv)
            .Concat(cipherBytes)
            .ToArray()
        );
        return result;
    }

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        var salt = Encoding.UTF8.GetString(fullCipher.Take(36).ToArray());
        var iv = fullCipher.Skip(36).Take(16).ToArray();
        var cipher = fullCipher.Skip(52).ToArray();
        var key = GetKey(salt);
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}