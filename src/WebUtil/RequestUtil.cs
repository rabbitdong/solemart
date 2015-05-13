using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Solemart.WebUtil
{
    public static class RequestUtil
    {
        /// <summary>
        /// Assert the request is from weixin.
        /// </summary>
        /// <param name="serverVariables"></param>
        /// <returns></returns>
        public static bool IsWeixinRequest(NameValueCollection serverVariables)
        {
            if (serverVariables.AllKeys.Contains("HTTP_USER_AGENT"))
            {
                if (serverVariables["HTTP_USER_AGENT"].Contains("MicroMessenger"))
                    return true;
            }

            return false;
        }
    }
}
