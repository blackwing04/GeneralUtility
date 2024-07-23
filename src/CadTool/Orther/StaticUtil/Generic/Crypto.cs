using StaticUtil.Models.DAO;
using System;
using System.IO;
using System.Security.Cryptography;

namespace StaticUtil.Generic
{
    /// <summary>
    /// 加解密服務層
    /// </summary>
    public class Crypto
    {
        private static readonly byte[] Key = GenerateRandomKey();
        private static readonly byte[] IV = GenerateRandomIV();

        /// <summary>
        /// 產生隨機的初始向量，長度為16。
        /// </summary>
        /// <returns>
        /// 返回一個新的隨機初始向量。
        /// </returns>
        private static byte[] GenerateRandomIV()
        {
            byte[] randomNumber = new byte[16]; // AES IV 長度
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
            }
            return randomNumber;
        }
        /// <summary>
        /// 產生隨機的密鑰，長度為32。
        /// </summary>
        /// <returns>
        /// 返回一個新的隨機密鑰。
        /// </returns>
        private static byte[] GenerateRandomKey()
        {
            byte[] randomNumber = new byte[32]; // AES IV 長度
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
            }

            return randomNumber;
        }
        /// <summary>
        /// 金鑰
        /// </summary>
        public static byte[] Key1 => Key;
        /// <summary>
        /// 向量
        /// </summary>
        public static byte[] IV1 => IV;

        /// <summary>
        /// 將提供的連接字串加密並產生對應的密鑰和初始向量。
        /// </summary>
        /// <param name="plainText">要加密的連接字串。</param>
        /// <returns>返回經過加密並以 Base64 格式表示的連接字串。</returns>
        public static string EncryptString(string plainText)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Key, IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)){
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }
        /// <summary>
        /// 依據連結字串，創建加密模型
        /// </summary>
        /// <returns>返回連結字串的加密模型</returns>
        public static CryptoConnectionStringModel CreatEncryptMode(string connectionString)
        {
            CryptoConnectionStringModel Encrypt = new CryptoConnectionStringModel(){
                K1=Convert.ToBase64String(Key1),
                V2=Convert.ToBase64String(IV1),
                CS=EncryptString(connectionString)
            };
            return Encrypt;
        }
        /// <summary>
        /// 使用提供的密鑰和初始向量解密指定的連接字串。
        /// </summary>
        /// <param name="connectionString">封裝好的加密連結字串</param>
        /// <returns>返回解密後的連接字串。 </returns>
        public static string DecryptString(CryptoConnectionStringModel connectionString)
        {
            string plaintext = null;
            var keyByte = Convert.FromBase64String(connectionString.K1);
            var ivByte = Convert.FromBase64String(connectionString.V2);
            using (Aes aesAlg = Aes.Create()) {
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(keyByte, ivByte);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(connectionString.CS))) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
