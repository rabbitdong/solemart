﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：AccessTokenHandlerWapper.cs
    文件功能描述：使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.WeixinAPI.Base.Exceptions;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Base;
using Solemart.WeixinAPI.Base.Entities;

namespace Solemart.WeixinAPI.CommonAPIs
{
    public static class AccessTokenHandlerWapper
    {
        /// <summary>
        /// 使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="fun">第一个参数为accessToken</param>
        /// <param name="retryIfFaild"></param>
        /// <returns></returns>
        public static T Do<T>(string appId, string appSecret, Func<string, T> fun, bool retryIfFaild = true) where T : WxJsonResult
        {
            T result = null;
            try
            {
                var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret, false);
                result = fun(accessToken);
            }
            catch (ErrorJsonResultException ex)
            {
                if (retryIfFaild && ex.JsonResult.errcode == WeixinReturnCode.获取access_token时AppSecret错误或者access_token无效)
                {
                    //尝试重新验证
                    var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret, true);
                    result = Do<T>(appId, appSecret, fun, false);
                }
            }
            return result;
        }
    }
}
