using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.SystemUtil
{
    /// <summary>
    /// 逗号分隔字符串处理工具类
    /// </summary>
    public static class DotStringUtil
    {
        /// <summary>
        /// 获取逗号分隔的最后一个字符串
        /// </summary>
        /// <param name="dotpartstring">字符串，形如abc,def,392,aaa</param>
        public static string GetLastDotCode(string dotpartstring)
        {
            if (string.IsNullOrWhiteSpace(dotpartstring))
                return string.Empty;

            string[] allparts = dotpartstring.Split(',');
            return allparts.Last();
        }

        public static bool IsStringInDotCode(string dotpartstring, string str)
        {
            string[] allparts = dotpartstring.Split(',');
            if (allparts == null || allparts.Length == 0)
                return false;

            return allparts.Contains(str);
        }

        /// <summary>
        /// 添加一新编号
        /// </summary>
        /// <param name="dotpartstring"></param>
        /// <param name="str"></param>
        /// <remarks>如果原编号中有该串，就把该串移到末尾，否则就加入末尾</remarks>
        public static string AddNewCode(string dotpartstring, string str)
        {
            if (string.IsNullOrWhiteSpace(dotpartstring))
                return str;

            string[] allparts = dotpartstring.Split(',');
            int len = allparts.Length, index = -1;

            //重新组成字符串
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; ++i)
            {
                if (allparts[i] != str)
                {
                    sb.AppendFormat("{0},", allparts[i]);
                }
                else
                    index = i;
            }

            //不管找到还是没找到，都加到末尾
            return sb.ToString() + str;
        }
    }
}
