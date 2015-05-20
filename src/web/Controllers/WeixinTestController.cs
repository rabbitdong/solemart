using SimLogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Solemart.Web.Controllers
{
    public class WeixinTestController : Controller
    {
        //
        // GET: /WeixinTest/

        /// <summary>
        /// The interface for validating from the weixin
        /// </summary>
        /// <returns></returns>
        /// signature  微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。  
        /// timestamp  时间戳  
        /// nonce  随机数  
        /// echostr  随机字符串  
        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            string token = "solemart";
            string[] tmpArr = { token, timestamp, nonce };
            Array.Sort(tmpArr);
            string tmpStr = tmpArr[0] + tmpArr[1] + tmpArr[2];
            string resultStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            Log.Instance.WriteLog(string.Format("ValidateWeixin: request[{0}], calculate[{1}]", tmpStr, resultStr));

            return Content(echostr);
        }

    }
}
