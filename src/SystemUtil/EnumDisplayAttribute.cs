using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Solemart.SystemUtil
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class EnumDisplayAttribute : Attribute
    {
        /// <summary>
        /// 要显示的名称
        /// </summary>
        public string DisplayStr;

        /// <summary>
        /// Constructor of EnumDisplayAttribute
        /// </summary>
        /// <param name="displayStr">The display string.</param>
        public EnumDisplayAttribute(string displayStr)
        {
            this.DisplayStr = displayStr;
        }
    }

    /// <summary>
    /// 枚举名称显示类
    /// </summary>
    public static class EnumDisplay
    {
        private static readonly string JsonFmt = "{{\"value\":\"{0}\",\"name\":\"{1}\"}}";

        /// <summary>
        /// 显示该枚举的显示内容
        /// </summary>
        /// <param name="myenum">要显示的枚举值</param>
        /// <returns>该枚举的显示内容</returns>
        public static string ToDisplayStr(this Enum myenum)
        {
            FieldInfo fieldInfo = myenum.GetType().GetField(myenum.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                return fieldInfo.Name;

            return descriptionAttributes[0].DisplayStr;
        }

        /// <summary>
        /// 获取该枚举的Json表达式字符串
        /// </summary>
        /// <param name="myenum">要显示的枚举值</param>
        /// <returns>Json表达式内容</returns>
        public static string ToJsonStr(this Enum myenum)
        {
            string showstr = string.Empty;
            var fieldInfo = myenum.GetType().GetField(myenum.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                showstr = fieldInfo.Name;
            else
                showstr = descriptionAttributes[0].DisplayStr;

            return string.Format(JsonFmt, (int)fieldInfo.GetValue(myenum), showstr);
        }
    }
}
