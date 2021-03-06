﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：GetMediaCountResultJson.cs
    文件功能描述：获取素材总数返回结果
    
    
    创建标识：Senparc - 20150324
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.WeixinAPI.AdvancedAPIs.GroupMessage;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Base.Entities;

namespace Solemart.WeixinAPI.AdvancedAPIs.Media
{
    /// <summary>
    /// 获取素材总数返回结果
    /// </summary>
    public class GetMediaCountResultJson : WxJsonResult
    {
        /// <summary>
        /// 语音总数量
        /// </summary>
        public int voice_count { get; set; }
        /// <summary>
        /// 视频总数量
        /// </summary>
        public int video_count { get; set; }
        /// <summary>
        /// 图片总数量
        /// </summary>
        public int image_count { get; set; }
        /// <summary>
        /// 图文总数量
        /// </summary>
        public int news_count { get; set; }
    }
}
