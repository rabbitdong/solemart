﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
  
    文件名：ReturnResult.cs
    文件功能描述：返回结果类型
    
    
    创建标识：Senparc - 20150319
----------------------------------------------------------------*/

using Solemart.WeixinAPI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.WeixinAPI.AppStore
{
   public class ReturnResult
    {
        /// <summary>
        /// 如果>0则进入某个APP状态，如果=0则维持当前状态不变，如果>0则退出某个App状态
        /// </summary>
       public AppStoreState AppStoreState { get; set; }
       /// <summary>
       /// 改变状态的AppId
       /// </summary>
       public int AppId { get; set; }
       /// <summary>
       /// 错误信息
       /// </summary>
       public string ErrorMessage { get; set; }
    }
}
