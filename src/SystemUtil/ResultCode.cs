using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.SystemUtil
{
    /// <summary>
    /// 各层传递的错误码
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// 结果成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 结果失败
        /// </summary>
        Error = 1,

        /// <summary>
        /// 某个字段重复
        /// </summary>
        DuplicatedField = 0x2001,

        /// <summary>
        /// 字段不合法
        /// </summary>
        ParamInvalid = 0x2002,

        /// <summary>
        /// 对象未找到
        /// </summary>
        ObjectNotFound = 0x2003,

        /// <summary>
        /// 存在相关对象（针对删除）
        /// </summary>
        ExistCorrelationObject = 0x2004,

        /// <summary>
        /// 无效操作，不能再改对象上执行该操作
        /// </summary>
        InvalidOperate = 0x2005,
    }
}
