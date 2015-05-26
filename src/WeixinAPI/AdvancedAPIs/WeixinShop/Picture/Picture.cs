using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Solemart.WeixinAPI.CommonAPIs;
using Solemart.WeixinAPI.Entities;

namespace Solemart.WeixinAPI.AdvancedAPIs
{
    /// <summary>
    /// 微小店接口，官方API：http://mp.weixin.qq.com/wiki/index.php?title=%E5%BE%AE%E4%BF%A1%E5%B0%8F%E5%BA%97%E6%8E%A5%E5%8F%A3
    /// </summary>
    public static class WeixinShopPicture
    {

        public static PictureResult GetByIdOrder(string accessToken, string fileName)
        {
            var urlFormat = "https://api.weixin.qq.com/merchant/common/upload_img?access_token={0}&filename={1}";
            var url = string.IsNullOrEmpty(accessToken) ? urlFormat : string.Format(urlFormat, accessToken, fileName);

            var json=new PictureResult();

            using (var fs = Solemart.WeixinAPI.Base.Helpers.FileHelper.GetFileStream(fileName))
            {
                var jsonText = Solemart.WeixinAPI.Base.HttpUtility.RequestUtility.HttpPost(url, null, fs);
                json = Solemart.WeixinAPI.Base.HttpUtility.PostMethod.GetResult<PictureResult>(jsonText);
            }
            return json;
        }
    }
}
