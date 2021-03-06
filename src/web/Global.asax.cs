﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.WebUtil;
using SimLogLib;
using Solemart.WeixinAPI.CommonAPIs;

namespace Solemart.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {

        private void Application_Start(object sender, EventArgs e)
        {
            ConfigSettings.LoadAppConfig();
            LogConfig.LogDir = Server.MapPath("~/Log");
            LogConfig.LogCapacityEachBlock = 500;

            //注册微信的AccessToken获取
            AccessTokenContainer.Register(ConfigSettings.TestWeixinAppID, ConfigSettings.TestWeixinAppSecret);

            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        /// <summary>
        /// 验证的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = Request.Cookies.Get(FormsAuthentication.FormsCookieName);
            if (cookie != null && cookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                int userid = Convert.ToInt32(ticket.UserData);
                HttpContext.Current.User = SolemartUserCache.GetUser(userid);
            }
            else
            {
                //如果没有Cookie，说明用户是非注册用户。就用匿名用户进行登陆。
                AccountUtil.Login(SolemartUserCache.GetUser(SolemartUser.DefaultAnonymousUserID));
            }
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            //请求js，css，.jpg, .png等忽略
            if (Request.RawUrl.Contains(".js") || Request.RawUrl.Contains(".css") || Request.RawUrl.Contains(".jpg") || Request.RawUrl.Contains(".png"))
                return;

            bool isFromWeixin = WebUtil.RequestUtil.IsWeixinRequest(Request.ServerVariables);
            Log.Instance.WriteLog(string.Format("[{0}],url[{1}],form[{2}],from[{3}],FromWeixin[{4}]",Request.HttpMethod, Request.RawUrl, Request.Form.GetLogString(), Request.UserHostAddress, isFromWeixin));
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            //Log.Instance.WriteError(Server.GetLastError().ToString());
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            //初始化会话用户的购物车对象
            //如果用户之前有未处理的购物车的商品项
            SolemartUser user = HttpContext.Current.User as SolemartUser;
            if (user.Cart.CartItems.Count > 0)
                user.SaveCart();

            AccountUtil.Logout();
        }
    }
}