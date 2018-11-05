using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Storage;
using Newtonsoft.Json;

namespace BoxsieApp.Core
{
    public static class BoxsieUtils
    {
        public static string CreateProgressBar(double percent, int steps)
        {
            var progress = Math.Floor((steps / 100d) * percent);
            var bar = "";

            for (var o = 0; o < steps; o++)
            {
                bar += progress > o
                    ? '#'
                    : '.';
            }

            return $"[{bar}]";
        }

        public static OS GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OS.Windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OS.OSX;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OS.Linux;

            throw new Exception("Operating system not supported");
        }

        public static string GetDefaultUserDataPath(string dirName)
        {
            var platform = GetPlatform();
            var path = "";

            switch (platform)
            {
                case OS.Windows:
                    path = Environment.GetEnvironmentVariable("LocalAppData");
                    break;
                case OS.OSX:
                    path = $"~/Library/Application Support/";
                    break;
                case OS.Linux:
                    path = $"Home/";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"{Path.Combine(path, dirName)}{Path.DirectorySeparatorChar}";
        }
        
        public static JsonSerializerSettings GetDefaultSerialiserSettings()
        {
            return new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc };
        }

        public static async Task<string> EncryptTextAsync(string text, string key)
        {
            key = PadKey(key);

            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    return null;
                
                using (var encryptor = aesAlg.CreateEncryptor(keyBytes, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                await swEncrypt.WriteAsync(text);
                            }
                        }
                        
                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[aesAlg.IV.Length + decryptedContent.Length];

                        Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, aesAlg.IV.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static async Task<string> DecryptTextAsync(string encryptedText, string key)
        {
            key = PadKey(key);

            var fullCipher = Convert.FromBase64String(encryptedText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    return null;
                
                using (var decryptor = aesAlg.CreateDecryptor(keyBytes, iv))
                {
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                return await srDecrypt.ReadToEndAsync();
                            }
                        }
                    }
                }
            }
        }

        public static byte[] EncryptSha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
                return hash.ComputeHash(messageBytes);
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static string PadKey(string key)
        {
            var keyBase = Cfg.GetConfig<GeneralConfig>().EncryptKeyBase;

            const int bitLen = 32;

            if (key.Length < bitLen)
                key = $"{key}{keyBase.Substring(0, bitLen - key.Length)}";
            else if (key.Length > bitLen)
                key = key.Substring(0, bitLen);

            return key;
        }
    }
}