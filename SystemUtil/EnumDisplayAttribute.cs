using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.SystemUtil
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class EnumDisplayAttribute : Attribute
    {
        /// <summary>
        /// 要显示的名称
        /// </summary>
        public string Display;
    }

    /// <summary>
    /// 枚举名称显示类
    /// </summary>
    public static class EnumDisplay
    {
        /// <summary>
        /// 显示该枚举的显示内容
        /// </summary>
        /// <param name="myenum">要显示的枚举值</param>
        /// <returns>该枚举的显示内容</returns>
        public static string ToDisplay(this Enum myenum)
        {
            var fieldInfo = myenum.GetType().GetField(myenum.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                return fieldInfo.Name;

            return descriptionAttributes[0].Display;
        }

        /// <summary>
        /// 获取该枚举的Json表达式字符串
        /// </summary>
        /// <param name="myenum">要显示的枚举值</param>
        /// <returns>Json表达式内容</returns>
        public static string ToJsonStr(this Enum myenum)
        {
            string showstr = null;
            var fieldInfo = myenum.GetType().GetField(myenum.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                showstr = fieldInfo.Name;
            else
                showstr = descriptionAttributes[0].Display;

            return string.Format("{{\"value\":\"{0}\",\"name\":\"{1}\"}}", (int)fieldInfo.GetValue(myenum), showstr);
        }
    }
}
