using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Xxx.EntityLib;
using Xxx.BusinessLib;
using Xxx.SystemUtil;
using Xxx.DataAccessLib;
using System.Web.Security;

namespace Xxx.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication {

        private void Application_Start(object sender, EventArgs e) {
            // Code that runs on application startup
            string constr = SystemManager.Instance.DecryptString(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["sq_adon888"].ConnectionString);
            SystemConnection.ConnectString = constr;
            LogManager.BaseDir = this.Server.MapPath("\\");

            SystemManager.Instance.InitAdminAccount();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_End(object sender, EventArgs e) {
            //  Code that runs on application shutdown

        }

        void Application_BeginRequest(object sender, EventArgs e) {
            LogManager.Instance.Log(string.Format("Request from:{0}\t{1}", Context.Request.UserHostAddress, Context.Request.RawUrl));
        }

        /// <summary>验证的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_AuthenticateRequest(object sender, EventArgs e) {
            HttpCookie cookie = Request.Cookies.Get(FormsAuthentication.FormsCookieName);
            if (cookie != null && cookie.Value != "") {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                LoginType login_type = LoginType.Local;
                string type_str = ticket.Name.Substring(0, 2);
                if (type_str == "QQ") {
                    login_type = LoginType.QQ;
                }
                string name_or_id = ticket.Name.Substring(3);

                LogManager.Instance.Log(string.Format("{0} Logon....", name_or_id));
                EntityLib.User user = null;
                if (login_type == LoginType.Local) {
                    user = BusinessLib.UserManager.Instance.GetUserByName(name_or_id);
                }
                else if (login_type == LoginType.QQ) {
                    user = BusinessLib.UserManager.Instance.GetUserByOpenId(name_or_id);
                }

                if (user == null)
                    user = EntityLib.User.Anonymous;

                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                        new System.Security.Principal.GenericIdentity(name_or_id), EntityLib.Role.GetRoleNames(user.Roles));
            }
        }

        void Application_Error(object sender, EventArgs e) {
            // Code that runs when an unhandled error occurs
            LogManager.Instance.Log(this.Context.Error.ToString());
        }

        void Session_Start(object sender, EventArgs e) {
            //初始化会话用户的购物车对象
            //如果用户之前有未处理的购物车的商品项
            if (Session["cart"] == null)
                Session["cart"] = new BusinessLib.Cart();

            if (HttpContext.Current.User.Identity.IsAuthenticated) {
                string uname = HttpContext.Current.User.Identity.Name;
                Xxx.EntityLib.User user = UserManager.Instance.GetUserByName(uname);
                if (Session["user"] == null)
                    Session.Add("user", user);
                else {
                    EntityLib.User preuser = Session["user"] as EntityLib.User;
                    if (preuser.UserID != user.UserID)
                        Session["user"] = user;
                }
            }
            else {
                if (Session["user"] == null)
                    Session.Add("user", EntityLib.User.Anonymous);
            }

            // 在用户开放浏览的时候，把用户导向到mobile版页面
            //HttpRequest httpRequest = HttpContext.Current.Request;
            //if (httpRequest.Browser.IsMobileDevice) {
            //    string path = httpRequest.Url.PathAndQuery;
            //    bool isOnMobilePage = path.StartsWith("/mobile/",
            //                           StringComparison.OrdinalIgnoreCase);
            //    if (!isOnMobilePage) {
            //        string redirectTo = "~/mobile/default.aspx";

            //        HttpContext.Current.Response.Redirect(redirectTo);
            //    }
            //}

        }

        void Session_End(object sender, EventArgs e) {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            //初始化会话用户的购物车对象
            //如果用户之前有未处理的购物车的商品项
            BusinessLib.Cart cart = Session["cart"] as BusinessLib.Cart;
            if (cart.Products.Count > 0)
                cart.Save(Session["user"] as EntityLib.User);

            this.Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            Response.Cookies.Add(cookie);

            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
              new System.Security.Principal.GenericIdentity(EntityLib.User.Anonymous.Name),
              EntityLib.Role.GetRoleNames(EntityLib.User.Anonymous.Roles));
        }
    }
}