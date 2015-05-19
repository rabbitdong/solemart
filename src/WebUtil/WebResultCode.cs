using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.WebUtil
{
    /// <summary>
    /// Web结果的状态码
    /// </summary>
    public enum WebResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 1,

        /// <summary>
        /// 会话已经过期
        /// </summary>
        SessionExpired = 0x1000,

        /// <summary>
        /// 验证码错误
        /// </summary>
        ValidateCodeError = 0x1001,

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        UserNameOrPwdError = 0x1004,

        /// <summary>
        /// 参数错误
        /// </summary>
        ParametersError = 0x2000,

        /// <summary>
        /// 文件太大
        /// </summary>
        FileTooLong = 0x2001,

        /// <summary>
        /// 输入不完整
        /// </summary>
        IncompleteInput = 0x2002,

        /// <summary>
        /// 重复字段错误
        /// </summary>
        DuplicatedField = 0x4000,

        /// <summary>
        /// 编号不存在
        /// </summary>
        CodeNotExist = 0x4001,
    }
}
