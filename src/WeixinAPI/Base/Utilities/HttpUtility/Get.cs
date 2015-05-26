﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：Get.cs
    文件功能描述：Get
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Solemart.WeixinAPI.Base.Entities;
using Solemart.WeixinAPI.Base.Exceptions;
using Newtonsoft.Json;

namespace Solemart.WeixinAPI.Base.HttpUtility
{
    public static class GetMethod
    {
        public static T GetJson<T>(string url, Encoding encoding = null)
        {
            string returnText = HttpUtility.RequestUtility.HttpGet(url, encoding);


            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                WxJsonResult errorResult = JsonConvert.DeserializeObject<WxJsonResult>(returnText);
                if (errorResult.errcode != WeixinReturnCode.Success)
                {
                    //发生错误
                    throw new ErrorJsonResultException(
                        string.Format("微信请求发生错误！错误代码：{0}，说明：{1}",
                                        (int)errorResult.errcode,
                                        errorResult.errmsg),
                                      null, errorResult);
                }
            }

            T result = JsonConvert.DeserializeObject<T>(returnText);

            return result;
        }

        public static void Download(string url, Stream stream)
        {
            WebClient wc = new WebClient();
            var data = wc.DownloadData(url);
            foreach (var b in data)
            {
                stream.WriteByte(b);
            }
        }
    }
}
