using System.IO;
using System.Security.Cryptography;
using System.Text;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes.CodeAuth;

namespace WPF.Admin.Themes.Helper {
    public class TextCodeHelper {
        public static string NoAuthorizationFile =
            ApplicationConfigConst.NoAuthorizationFile;

        public static void NoAuthorizationRequired() {
            if (System.IO.File.Exists(NoAuthorizationFile))
            {
                return;
            }

            var sri = ApplicationUtils
                .FindApplicationResourceStreamInfo("WPF.Admin.Themes", "Resources/authcode.dll");

            if (sri == null)
            {
                return;
            }

            using StreamReader reader = new StreamReader(sri.Stream);
            try
            {
                string result = reader.ReadToEnd();
                var result2 = Decrypt(result);
                if (ApplicationCodeAuth.nasduabwduadawdb(result2) == ApplicationConfigConst.Code)
                {
                    System.IO.File.WriteAllText(NoAuthorizationFile, result2);
                }

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError(ex.Message, ex);
            }
        }


        private static readonly string Key = ApplicationAuthModule.DllCreateTime.TimeYearMonthDayHourString();


        public static string Encrypt(string? plainText = null, string key = "") {
            if (string.IsNullOrEmpty(key))
            {
                key = Key; 
            }

            key += "14332231";

            if (string.IsNullOrEmpty(plainText))
            {
                plainText = ApplicationConfigConst._nnnnnnnnnnnnnn; 
            }

          
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                var iv = aes.IV;
                
                var aesKey = DeriveKey(key, aes.KeySize);
                
                byte[] encryptedBytes;
                using (var encryptor = aes.CreateEncryptor(aesKey, iv))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    encryptedBytes = ms.ToArray();
                }

             
                var combined = new byte[2 + iv.Length + encryptedBytes.Length];
              
                BitConverter.GetBytes((short)iv.Length).CopyTo(combined, 0);
                iv.CopyTo(combined, 2);
                encryptedBytes.CopyTo(combined, 2 + iv.Length);

                return Convert.ToBase64String(combined);
            }
        }

        private static byte[] DeriveKey(string key, int keySize) {
            using (var sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha256.ComputeHash(keyBytes);

              
                int bytesNeeded = keySize / 8;
                if (hash.Length > bytesNeeded)
                {
                    byte[] result = new byte[bytesNeeded];
                    Array.Copy(hash, result, bytesNeeded);
                    return result;
                }

                return hash;
            }
        }

        public static string Decrypt(string cipherText, string key = "") {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            try
            {
              
                if (string.IsNullOrEmpty(key))
                {
                    key = Key; 
                }

                key += "14332231";
                
                var combined = Convert.FromBase64String(cipherText);
                
                short ivLength = BitConverter.ToInt16(combined, 0); 
                var iv = new byte[ivLength];
                var encryptedBytes = new byte[combined.Length - 2 - ivLength];

                Array.Copy(combined, 2, iv, 0, ivLength);
                Array.Copy(combined, 2 + ivLength, encryptedBytes, 0, encryptedBytes.Length);
                
                using (var aes = Aes.Create())
                {
                    var aesKey = DeriveKey(key, aes.KeySize);

                    // 解密
                    using (var decryptor = aes.CreateDecryptor(aesKey, iv))
                    using (var ms = new MemoryStream(encryptedBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError($"解密失败: {ex.Message}");
                return string.Empty;
            }
        }
    }
}