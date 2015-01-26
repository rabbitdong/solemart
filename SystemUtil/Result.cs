using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobo91.Sdk.Dev.SystemUtil
{
    /// <summary>
    /// 业务方法结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public static Result<string> NormalErrorResult = new Result<string> { ResultCode= ResultCode.Error };
        public static Result<string> SuccessResult = new Result<string> { ResultCode = ResultCode.Success };
        public static Result<string> DuplicatedField = new Result<string> { ResultCode = ResultCode.DuplicatedField };
        public static Result<string> ParamInvalid = new Result<string> { ResultCode = ResultCode.ParamInvalid };
        public static Result<string> ObjectNotFound = new Result<string> { ResultCode = ResultCode.ObjectNotFound };
        public static Result<string> ExistCorrelationObject = new Result<string> { ResultCode = ResultCode.ExistCorrelationObject };
        public static Result<string> InvalidOperate = new Result<string> { ResultCode = ResultCode.InvalidOperate, ResultMessage = "无效操作" };

        /// <summary>
        /// 结果编码
        /// </summary>
        public ResultCode ResultCode { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// 结果数据
        /// </summary>
        public T ResultData { get; set; }

        public void SetInfo(ResultCode resultCode, string resultMessage, T resultData)
        {
            this.ResultCode = resultCode;
            this.ResultMessage = resultMessage;
            this.ResultData = resultData;
        }
    }
}
