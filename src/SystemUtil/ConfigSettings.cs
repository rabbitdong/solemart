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
        /// Load the system configuration
        /// </summary>
        public static void LoadAppConfig()
        {
            if(ConfigurationManager.AppSettings["ConnectString"] != null)
                ConnectionString = EncryptUtil.DecryptString(ConfigurationManager.AppSettings["ConnectString"]);
        }
    }
}
