using System;
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

namespace Solemart.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {

        private void Application_Start(object sender, EventArgs e)
        {
            ConfigSettings.LoadAppConfig();
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
                LoginType login_type = LoginType.Local;
                string type_str = ticket.Name.Substring(0, 2);
                if (type_str == "QQ")
                {
                    login_type = LoginType.QQ;
                }
                int userid = Convert.ToInt32(ticket.UserData);
                HttpContext.Current.User = SolemartUserCache.GetUser(userid);
            }
            else
            {
                HttpContext.Current.User = SolemartUser.Anonymous;
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
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

            this.Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            Response.Cookies.Add(cookie);
        }
    }
}