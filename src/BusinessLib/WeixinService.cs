using Newtonsoft.Json;
using SimLogLib;
using Solemart.SystemUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Solemart.Business
{
    /// <summary>
    /// The weixin service class.
    /// </summary>
    public class WeixinService
    {
        private Timer getTokenTimer = null;
        private static string weixinAccessToken;

        public WeixinService()
        {
            getTokenTimer = new Timer(InternalGetAccessToken, null, 0, 7000000);
        }

        private void InternalGetAccessToken(object obj)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                ConfigSettings.TestWeixinAppID, ConfigSettings.TestWeixinAppSecret);
            WebClient client = new WebClient();
            string result = client.DownloadString(url);
            Log.Instance.WriteLog(string.Format("InternalGetAccessToken get token:[{0}]", result));
            var jsonResult = JsonConvert.DeserializeAnonymousType(result, new {accessToken = "" });
            weixinAccessToken = jsonResult.accessToken;
        }

        /// <summary>
        /// Get the access token for the weixin client.
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            return weixinAccessToken;
        }
    }
}
