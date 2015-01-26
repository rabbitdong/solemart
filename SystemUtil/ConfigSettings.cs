using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Solemart.SystemUtil
{
    public class ConfigSettings
    {
        /// <summary>
        /// 设置系统的连接字符串，下面是默认值
        /// </summary>
        public static string ConnectionString = "Server=192.168.189.19;Database=Mobo91_SDK_DEV;user id=yanfa;password=ftpuseryanfa;";

        /// <summary>
        /// 最大上传的图片大小
        /// </summary>
        public static int MaxUploadImgLen = 1;   // 1表示1M

        /// <summary>
        /// 通用的图片保存地址
        /// </summary>
        public static string ImgSavePath = "E:/Projects/ForeignSDK/Dev/images/";

        /// <summary>
        /// 返回的图片的网站地址
        /// </summary>
        public static string ImgSite = "http://192.168.255.204:807/";

        /// <summary>
        /// DEV的前台站点
        /// </summary>
        public static string Site = "http://192.168.255.204:805";

        /// <summary>
        /// 用于本站点加密、MD5处理等KEY
        /// </summary>
        public static string SITE_KEY = "6E153263-55A4-4E5B-B3D5-482685412328";

        public static void LoadAppConfig()
        {
            if(ConfigurationManager.AppSettings["Mobo91_SDK_DEV_DB"] != null)
                ConnectionString = DescHelper.Decrypt(ConfigurationManager.AppSettings["Mobo91_SDK_DEV_DB"]);
            
            ImgSite = ConfigurationManager.AppSettings["DevImgSite"] ?? ImgSite;

            ImgSavePath = ConfigurationManager.AppSettings["ImgSavePath"] ?? ImgSavePath;

            Site = ConfigurationManager.AppSettings["Site"] ?? Site;
        }
    }
}
