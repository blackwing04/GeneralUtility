using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Generic.StaticUtil
{
    /// <summary>
    /// 加解密服務層
    /// </summary>
    public static class CryptoHelper
    {
        #region 加密
        /// <summary>
        /// 用於 AES 加密的金鑰值（32 位元）
        /// </summary>
        public static byte[] Key1 => Key;
        /// <summary>
        /// 用於 AES 加密的初始向量值（16 位元）
        /// </summary>
        public static byte[] IV1 => IV;
        /// <summary>
        /// 將提供的連接字串加密並產生對應的密鑰和初始向量。
        /// </summary>
        /// <param name="plainText">要加密的連接字串。</param>
        /// <returns>
        /// 返回經過加密並以 Base64 格式表示的連接字串。
        /// </returns>
        public static string EncryptString(string plainText)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create()) {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Key, IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// 將給定的密鑰進行加密。
        /// </summary>
        /// <param name="key">需要加密的密鑰。</param>
        /// <returns>
        /// 返回一個加密後的密鑰字符串。
        /// </returns>
        public static string Encrypted(string key)
        {
            List<string> keys = key.Split('-').ToList();
            string EncryptedKeys = keys[3] + keys[1] + keys[4] + keys[2] + keys[0];
            Dictionary<char, char> mapping = new Dictionary<char, char>()
            {
                { '0', '7' }, { '1', '4' }, { '2', '9' }, { '3', '6' },
                { '4', '1' }, { '5', '8' }, { '6', '3' }, { '7', '0' },
                { '8', '5' }, { '9', '2' },
                { 'A', 'N' }, { 'B', 'K' }, { 'C', 'P' }, { 'D', 'H' },
                { 'E', 'S' }, { 'F', 'M' }, { 'G', 'R' }, { 'H', 'E' },
                { 'I', 'W' }, { 'J', 'T' }, { 'K', 'Z' }, { 'L', 'Q' },
                { 'M', 'Y' }, { 'N', 'F' }, { 'O', 'V' }, { 'P', 'C' },
                { 'Q', 'L' }, { 'R', 'X' }, { 'S', 'J' }, { 'T', 'B' },
                { 'U', 'O' }, { 'V', 'G' }, { 'W', 'I' }, { 'X', 'D' },
                { 'Y', 'A' }, { 'Z', 'U' }
            };

            char[] result = EncryptedKeys.ToUpper().ToCharArray();

            for (int i = 0; i < result.Length; i++) {
                if (mapping.ContainsKey(result[i])) {
                    result[i] = mapping[result[i]];
                }
            }
            return new string(result);
        }

        /// <summary>
        /// 通過加鹽Hash方式對密碼進行加密。
        /// </summary>
        /// <param name="password">明文密碼。</param>
        /// <param name="salt">鹽值。</param>
        /// <returns>
        /// 返回一個加密後的密碼字符串。
        /// </returns>
        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create()) {
                var saltedPassword = password + salt;
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword)));
            }
        }
        /// <summary>
        /// 生成一個隨機Salt，長度為16。
        /// </summary>
        /// <returns>
        /// 返回一個不以 'error' 開頭的隨機Salt字符串。
        /// </returns>
        public static string GenerateSalt()
        {
            const int SaltSize = 16; // 128 bit 
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                var salt = new byte[SaltSize];
                //確保產生出來的Salt開頭不為Error以利後續辨識錯誤發生
                string saltStr;
                do {
                    rng.GetBytes(salt);
                    saltStr = Convert.ToBase64String(salt);
                } while (saltStr.StartsWith("error"));
                return saltStr;
            }
        }
        #endregion 加密

        #region 解密
        /// <summary>
        /// 使用提供的密鑰和初始向量解密指定的連接字串。
        /// </summary>
        /// <param name="cipherText">已經經過加密的連接字串。</param>
        /// <param name="key">用於解密的密鑰。</param>
        /// <param name="iv">用於解密的初始向量。</param>
        /// <returns>
        /// 返回解密後的連接字串。
        /// </returns>
        public static string DecryptString(string cipherText, string key, string iv)
        {
            string plaintext = null;
            var keyByte = Convert.FromBase64String(key);
            var ivByte = Convert.FromBase64String(iv);
            using (Aes aesAlg = Aes.Create()) {
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(keyByte, ivByte);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText))) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        /// <summary>
        /// 將給指定的令牌進行解密。
        /// </summary>
        /// <param name="encryptedToken">令牌</param>
        /// <param name="salt">用於解密的Salt</param>
        /// <returns>
        /// 返回一個解密後的金鑰。
        /// </returns>
        public static string DecryptKey(string encryptedToken, string salt)
        {
            Dictionary<char, char> reverseMapping = new Dictionary<char, char>()
            {
                { '7', '0' }, { '4', '1' }, { '9', '2' }, { '6', '3' },
                { '1', '4' }, { '8', '5' }, { '3', '6' }, { '0', '7' },
                { '5', '8' }, { '2', '9' },
                { 'N', 'A' }, { 'K', 'B' }, { 'P', 'C' }, { 'H', 'D' },
                { 'S', 'E' }, { 'M', 'F' }, { 'R', 'G' }, { 'E', 'H' },
                { 'W', 'I' }, { 'T', 'J' }, { 'Z', 'K' }, { 'Q', 'L' },
                { 'Y', 'M' }, { 'F', 'N' }, { 'V', 'O' }, { 'C', 'P' },
                { 'L', 'Q' }, { 'X', 'R' }, { 'J', 'S' }, { 'B', 'T' },
                { 'O', 'U' }, { 'G', 'V' }, { 'I', 'W' }, { 'D', 'X' },
                { 'A', 'Y' }, { 'U', 'Z' }
            };

            //使用反向映射來解密字符。
            StringBuilder decryptedString = new StringBuilder();
            foreach (char c in encryptedToken)
            {
                if (reverseMapping.ContainsKey(c))
                {
                    decryptedString.Append(reverseMapping[c]);
                }
                else
                {
                    decryptedString.Append(c); // 如果該字符不在映射中，保持它不變。
                }
            }
            //恢復GUID段落的順序。
            List<string> decryptedKeysList = new List<string>()
            {
                decryptedString.ToString().Substring(24, 8), // 第5部分
                decryptedString.ToString().Substring(4, 4), // 第2部分
                decryptedString.ToString().Substring(20, 4), // 第4部分
                decryptedString.ToString().Substring(0, 4), // 第1部分
                decryptedString.ToString().Substring(8, 12) // 第3部分
            };
            var saltedPassword = string.Join("-", decryptedKeysList) + salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }
        /// <summary>
        /// 將給指定的令牌進行解密。
        /// </summary>
        /// <param name="encryptedToken">令牌</param>
        /// <param name="salt">用於解密的Salt</param>
        /// <returns>
        /// 返回一個解密後的金鑰。
        /// </returns>
        public static string DecryptKey(string encryptedToken)
        {
            Dictionary<char, char> reverseMapping = new Dictionary<char, char>()
            {
                { '7', '0' }, { '4', '1' }, { '9', '2' }, { '6', '3' },
                { '1', '4' }, { '8', '5' }, { '3', '6' }, { '0', '7' },
                { '5', '8' }, { '2', '9' },
                { 'N', 'A' }, { 'K', 'B' }, { 'P', 'C' }, { 'H', 'D' },
                { 'S', 'E' }, { 'M', 'F' }, { 'R', 'G' }, { 'E', 'H' },
                { 'W', 'I' }, { 'T', 'J' }, { 'Z', 'K' }, { 'Q', 'L' },
                { 'Y', 'M' }, { 'F', 'N' }, { 'V', 'O' }, { 'C', 'P' },
                { 'L', 'Q' }, { 'X', 'R' }, { 'J', 'S' }, { 'B', 'T' },
                { 'O', 'U' }, { 'G', 'V' }, { 'I', 'W' }, { 'D', 'X' },
                { 'A', 'Y' }, { 'U', 'Z' }
            };

            //使用反向映射來解密字符。
            StringBuilder decryptedString = new StringBuilder();
            foreach (char c in encryptedToken)
            {
                if (reverseMapping.ContainsKey(c))
                {
                    decryptedString.Append(reverseMapping[c]);
                }
                else
                {
                    decryptedString.Append(c); // 如果該字符不在映射中，保持它不變。
                }
            }
            //恢復GUID段落的順序。
            List<string> decryptedKeysList = new List<string>()
            {
                decryptedString.ToString().Substring(24, 8), // 第5部分
                decryptedString.ToString().Substring(4, 4), // 第2部分
                decryptedString.ToString().Substring(20, 4), // 第4部分
                decryptedString.ToString().Substring(0, 4), // 第1部分
                decryptedString.ToString().Substring(8, 12) // 第3部分
            };
            var saltedPassword = string.Join("-", decryptedKeysList);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }
        #endregion 解密

        #region 其他方法
        /// <summary>
        /// 返回加密字串跟加密金鑰等等的部分
        /// </summary>
        /// <returns>返回加密字段</returns>
        public static List<string> ShowEncrypt(string connectionString)
        {
            List<string> Encrypt = new List<string>(){
                $"K1:{Convert.ToBase64String(Key1)}" ,
                $"V2:{Convert.ToBase64String(IV1)}",
                $"CS:{EncryptString(connectionString)}"
            };
            return Encrypt;
        }
        #endregion 其他方法

        #region 私有變數(產生隨機碼)
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
                rng.GetBytes(randomNumber); // 使用 GetBytes 生成隨機數據
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
                rng.GetBytes(randomNumber); // 使用 GetBytes 生成隨機數據
            }
            return randomNumber;
        }
        #endregion 私有變數(產生隨機碼)
    }
}
