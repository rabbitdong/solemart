using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Solemart.SystemUtil
{
    /// <summary>
    /// System configuration support class
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>
        /// 设置系统的连接字符串，下面是默认值
        /// </summary>
        public static string ConnectionString = "server=localhost;database=solemart;uid=root;password=;";

        /// <summary>
        /// 最大上传的图片大小
        /// </summary>
        public static int MaxUploadImgLen = 1;   // 1表示1M

        /// <summary>
        /// 通用的图片保存地址
        /// </summary>
        public static string ImgSavePath = string.Empty;

        /// <summary>
        /// The local web site url.
        /// </summary>
        public static string SiteUrl = string.Empty;

        /// <summary>
        /// The token of weixin.
        /// </summary>
        public static string WeixinToken = "ledao";

        /// <summary>
        /// The weixin appid.
        /// </summary>
        public static string WeixinAppID = "wx076fc04f7025fe41";

        /// <summary>
        /// The weixin app secret.
        /// </summary>
        public static string WeixinAppSecret = "be31d106c801d07e7d0ed0417d822d64";

        /// <summary>
        /// The weixin test appid.
        /// </summary>
        public static string TestWeixinAppID = "wxb380b0cdc25542ac";

        /// <summary>
        /// The weixin test app secret.
        /// </summary>
        public static string TestWeixinAppSecret = "a84c7803c508474c4936c7942bd1f8e0";

        /// <summary>
        /// The AES encrypt key used by weixin.
        /// </summary>
        public static string WeixinEncodingAESKey = "A0KBv3U8ixCBNlsHuIQzOBTZ4TNQJKCGWLub23tNPkK";

        /// <summary>
        /// The time out of the weixin
        /// </summary>
        public static readonly int WeixinPostTimeOut = 30;

        /// <summary>
        /// Load the system configuration
        /// </summary>
        public static void LoadAppConfig()
        {
            if(ConfigurationManager.AppSettings["ConnectString"] != null)
                ConnectionString = EncryptUtil.DecryptString(ConfigurationManager.AppSettings["ConnectString"]);
        }
    }
}
