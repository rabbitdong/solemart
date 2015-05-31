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
using SimLogLib;

namespace Solemart.WeixinAPI.Base.HttpUtility
{
    public static class GetMethod
    {
        public static T GetJson<T>(string url, Encoding encoding = null) where T : WxJsonResult, new()
        {
            string returnText = RequestUtility.HttpGet(url, encoding);

            T result = new T();
            if (string.IsNullOrEmpty(returnText))
            {
                //可能发生错误
                WxJsonResult errorResult = new WxJsonResult();
                result.errcode = WeixinReturnCode.Exception;
                result.errmsg = "获取内容是空！";
            }
            else if (returnText.Contains("errcode"))
            {
                //可能发生错误
                WxJsonResult errorResult = JsonConvert.DeserializeObject<WxJsonResult>(returnText);
                Log.Instance.WriteError(string.Format("Get return error[{0}], errorcode[{1}], errormsg[{2}]", returnText, errorResult.errcode, errorResult.errmsg));
                if (errorResult != null) {
                    result.errcode = errorResult.errcode;
                    result.errmsg = errorResult.errmsg;
                }
                else
                {
                    result.errcode = WeixinReturnCode.Exception;
                    result.errmsg = string.Format("返回的内容[{0}]非法", returnText);
                }
            }
            else
            {
                result = JsonConvert.DeserializeObject<T>(returnText);
                result.errcode = WeixinReturnCode.Success;
                result.errmsg = "ok";
            }
            
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
