using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xxx.EntityLib {
    /// <summary>用于枚举的字符串表达式
    /// </summary>
    public class StringValueAttribute : Attribute {
        public string StringValue { get; protected set; }

        public StringValueAttribute(string value) {
            this.StringValue = value;
        }
    }

    /// <summary>工具类
    /// </summary>
    public static class Util{
        /// <summary>获取该枚举的字符串值
        /// </summary>
        /// <param name="val">枚举值</param>
        /// <returns>获取该枚举的字符串值</returns>
        public static string GetStringValue(this Enum value) {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : value.ToString();
        }
    }
}
