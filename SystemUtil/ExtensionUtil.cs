using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Solemart.SystemUtil
{
    public static class ExtensionUtil
    {
        /// <summary>
        /// 把名称/值对的项进行组合成一个字符串
        /// </summary>
        /// <param name="collection">要连接的名称/值对集合</param>
        public static string JoinAllValues(this NameValueCollection collection)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in collection)
            {
                sb.AppendFormat("{0}={1}&", key, collection[key]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取字符串的Base64编码
        /// </summary>
        /// <param name="sourceString">要编码的字符串</param>
        /// <param name="encoder">字符串使用的编码格式</param>
        public static string ToBase64String(this string sourceString, Encoding encoder)
        {
            byte[] data = encoder.GetBytes(sourceString);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 获取字符串的Base64编码(经过UTF8编码)
        /// </summary>
        /// <param name="sourceString">要编码的字符串</param>
        public static string ToBase64String(this string sourceString)
        {
            byte[] data = Encoding.UTF8.GetBytes(sourceString);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 获取字符串的MD5签名
        /// </summary>
        /// <param name="sourceString">要签名的字符串</param>
        /// <param name="encoder">字符串使用的编码格式</param>
        public static string ToMD5(this string sourceString, Encoding encoder)
        {
            MD5 MD5 = new MD5CryptoServiceProvider();
            byte[] data = MD5.ComputeHash(encoder.GetBytes(sourceString));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取字符串的MD5签名(经过UTF8编码)
        /// </summary>
        /// <param name="sourceString">要签名的字符串</param>
        public static string ToMD5(this string sourceString)
        {
            MD5 MD5 = new MD5CryptoServiceProvider();
            byte[] data = MD5.ComputeHash(Encoding.UTF8.GetBytes(sourceString));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}
