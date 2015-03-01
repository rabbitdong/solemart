using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.SystemUtil;
using Newtonsoft.Json;

namespace Solemart.WebUtil
{
    /// <summary>
    /// 服务端返回给Web的数据格式
    /// </summary>
    public enum ResultDataFormat
    {
        /// <summary>
        /// 结果为普通字符串
        /// </summary>
        [EnumDisplay(DisplayStr="Plan")]
        Plan,

        /// <summary>
        /// 结果表示为Json格式
        /// </summary>
        [EnumDisplay(DisplayStr="Json")]
        Json,

        /// <summary>
        /// 结果表示为Html格式
        /// </summary>
        [EnumDisplay(DisplayStr="Html")]
        Html,

        /// <summary>
        /// 结果表示为Xml格式
        /// </summary>
        [EnumDisplay(DisplayStr="Xml")]
        Xml
    }


    /// <summary>
    /// Web结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebResult<T>
    {
        /// <summary>
        /// 结果编码
        /// </summary>
        public WebResultCode ResultCode { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// 结果的数据格式
        /// </summary>
        public ResultDataFormat Format { get; set; }

        public static WebResult<string> SessionExpired = new WebResult<string> { ResultCode= WebResultCode.SessionExpired, ResultMessage = "对不起，您的登陆已超时！" };
        public static WebResult<string> SuccessResult = new WebResult<string> { ResultCode = WebResultCode.Success, ResultMessage = "成功", ResultData = "ok" };
        public static WebResult<string> ValideCodeErrorResult = new WebResult<string> { ResultCode = WebResultCode.ValidateCodeError, ResultMessage = "验证码无效", ResultData = "validate code error!" };
        public static WebResult<string> UserNameOrPasswordErrorResult = new WebResult<string> { ResultCode = WebResultCode.UserNameOrPwdError, ResultMessage = "用户名或密码错误", ResultData = "error user name or password!" };
        public static WebResult<string> ParameterErrorResult = new WebResult<string> { ResultCode = WebResultCode.ParametersError, ResultMessage = "参数错误", ResultData = "parameter error!" };
        public static WebResult<string> FileTooLongResult = new WebResult<string> { ResultCode = WebResultCode.FileTooLong, ResultMessage = "文件太大", ResultData = "1M" };
        public static WebResult<string> NormalErrorResult = new WebResult<string>() { ResultCode = WebResultCode.Error, ResultMessage = "发生错误" };

        /// <summary>
        /// 结果数据
        /// </summary>
        public T ResultData { get; set; }

        public void SetInfo(WebResultCode resultCode, string resultMessage, T resultData, ResultDataFormat format = ResultDataFormat.Plan)
        {
            this.ResultCode = resultCode;
            this.ResultMessage = resultMessage;
            this.Format = format;
            this.ResultData = resultData;
        }

        /// <summary>
        /// 获取结果数据的表示
        /// </summary>
        public string ResponseString
        {
            get
            {
                //如果是json格式，就进行序列化
                string resultString = string.Empty;
                if (ResultData == null)
                    resultString = "";
                else if (Format == ResultDataFormat.Json)
                    resultString = JsonConvert.SerializeObject(ResultData);
                else
                    resultString = ResultData.ToString();

                return string.Format("{0}|{1}|{2}|{3}", (int)ResultCode, ResultMessage, Format.ToDisplayStr(), resultString.ToBase64String());
            }
        }
    }
}
