﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：UploadImageResultJson.cs
    文件功能描述：门店 上传图片返回结果
    
    
    创建标识：Senparc - 20150513
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Base.Entities;

namespace Solemart.WeixinAPI.AdvancedAPIs.Poi
{
    /// <summary>
    /// 上传图片返回结果
    /// </summary>
    public class UploadImageResultJson : WxJsonResult
    {
        /// <summary>
        /// 上传成功后图片的链接
        /// </summary>
        public string url { get; set; }
    }
}
