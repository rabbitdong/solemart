﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：CommonApi.cs
    文件功能描述：通用接口(用于和微信服务器通讯，一般不涉及自有网站服务器的通讯)
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
    
    修改标识：Senparc - 20150330
    修改描述：获取调用微信JS接口的临时票据中的AccessToken添加缓存
    
    修改标识：Senparc - 20150401
    修改描述：添加公众号第三方平台获取授权码接口
    
    修改标识：Senparc - 20150430
    修改描述：公众号第三方平台分离
----------------------------------------------------------------*/

/*
    API：http://mp.weixin.qq.com/wiki/index.php?title=%E6%8E%A5%E5%8F%A3%E6%96%87%E6%A1%A3&oldid=103
    
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Solemart.WeixinAPI.Base.Helpers;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Entities.Menu;
using Solemart.WeixinAPI.Helpers;
using Solemart.WeixinAPI.Base.HttpUtility;

namespace Solemart.WeixinAPI.CommonAPIs
{
    /// <summary>
    /// 通用接口
    /// 通用接口用于和微信服务器通讯，一般不涉及自有网站服务器的通讯
    /// </summary>
    public partial class CommonApi
    {
        /// <summary>
        /// 获取凭证接口
        /// </summary>
        /// <param name="grant_type">获取access_token填写client_credential</param>
        /// <param name="appid">第三方用户唯一凭证</param>
        /// <param name="secret">第三方用户唯一凭证密钥，既appsecret</param>
        /// <returns></returns>
        public static AccessTokenResult GetToken(string appid, string secret, string grant_type = "client_credential")
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}",
                                    grant_type, appid, secret);

            AccessTokenResult result = GetMethod.GetJson<AccessTokenResult>(url);
            return result;
        }

        /// <summary>
        /// 用户信息接口
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static WeixinUserInfoResult GetUserInfo(string accessToken, string openId)
        {
            var url = string.Format("http://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}",
                                    accessToken, openId);
            WeixinUserInfoResult result = GetMethod.GetJson<WeixinUserInfoResult>(url);
            return result;
        }

        /// <summary>
        /// 获取调用微信JS接口的临时票据
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static JsApiTicketResult GetTicket(string appId, string secret, string type = "jsapi")
        {
            var accessToken = AccessTokenContainer.TryGetToken(appId, secret);

            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type={1}",
                                    accessToken, type);

            JsApiTicketResult result = GetMethod.GetJson<JsApiTicketResult>(url);
            return result;
        }
    }
}
