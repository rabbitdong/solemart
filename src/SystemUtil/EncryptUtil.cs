using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Solemart.SystemUtil
{
    /// <summary>
    /// The util for encrypting or decrypting the string.
    /// </summary>
    public class EncryptUtil
    {
        private static byte[] key = { 0x10, 0x35, 0x28, 0x84, 0xc5, 0x02, 0x3a, 0x7f };
        private static byte[] IV = { 0x51, 0x73, 0xa1, 0x1a, 0x07, 0x42, 0x86, 0xce };

        private static const int PWD_CHAR_LEN = 62; //数字+小写+大写 = 62个字符
        private static byte[] pwdchars = null;
        private static Random rand = new Random();

        static EncryptUtil()
        {
            pwdchars = new byte[PWD_CHAR_LEN];

            int num = 0;
            for (int i = 0; i < 10; ++i)
                pwdchars[num++] = (byte)('0' + i);

            for (int j = 'a'; j <= 'z'; ++j)
                pwdchars[num++] = (byte)j;

            for (int j = 'A'; j <= 'Z'; ++j)
                pwdchars[num++] = (byte)j;
        }

        /// <summary>
        /// 对字符串进行加密操作
        /// </summary>
        /// <param name="planttext">要加密的明文</param>
        /// <returns>加密后的密文</returns>
        public static string EncryptString(string planttext) {
            DESCryptoServiceProvider crypto = new DESCryptoServiceProvider();
            crypto.Key = key;
            crypto.IV = IV;

            byte[] plant_data = Encoding.UTF8.GetBytes(planttext);

            StringBuilder ciphertext = new StringBuilder();
            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, crypto.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(plant_data, 0, plant_data.Length);
                cs.FlushFinalBlock();

                byte[] cipher_data = ms.ToArray();
                foreach (byte cp in cipher_data)
                    ciphertext.AppendFormat("{0:X2}", cp);
            }

            return ciphertext.ToString();
        }

        /// <summary>
        /// 对字符串进行解密
        /// </summary>
        /// <param name="ciphertext">要解密的密文</param>
        /// <returns>解密后的明文</returns>
        public static string DecryptString(string ciphertext)
        {
            DESCryptoServiceProvider crypto = new DESCryptoServiceProvider();
            crypto.Key = key;
            crypto.IV = IV;

            //由于密文是16进制编码，所有字节长度是1/2
            int length = ciphertext.Length / 2;
            byte[] cipher_data = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                cipher_data[i] = (byte)(Convert.ToInt32(ciphertext.Substring(i * 2, 2), 16));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, crypto.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(cipher_data, 0, length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Get the hash of the password
        /// </summary>
        /// <param name="pwd">原始字符串</param>
        /// <returns></returns>
        public static string GetHashPwd(string pwd)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(pwd);
            byte[] hashed_bytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashed_bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random password char sequence
        /// </summary>
        /// <returns></returns>
        /// <remarks>生成的8位密码由10个数字、32个小写字母、32个大写字母组成</remarks>
        public static string GenerateRandomPassword()
        {
            int pwd_len = 8;
            byte[] pwd = new byte[pwd_len];

            for (int i = 0; i < pwd_len; ++i)
            {
                pwd[i] = pwdchars[rand.Next(PWD_CHAR_LEN)];
            }

            return Encoding.ASCII.GetString(pwd);
        }
    }
}
