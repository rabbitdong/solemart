using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Solemart.WebUtil
{
    public static class AccountUtil
    {
        #region 用户登录的方法
        /// <summary>
        /// 用户通过Email和密码登陆系统
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pwdmd5"></param>
        /// <param name="isPersist"></param>
        public static CommonDevUser Login(string email, string pwd, bool isPersist)
        {
            CommonDevUser user = UserManager.Login(0, email);
            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, email,
                    DateTime.Now, DateTime.Now.AddMinutes(30), isPersist, user.UserID.ToString());
                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                if (isPersist)
                    authCookie.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Current.Response.Cookies.Add(authCookie);
                HttpContext.Current.User = user;
            }
            return user;

            UserManager um = UserManager.Instance;

            HttpContext ctx = HttpContext.Current;
            User user = um.OnLogin(name, pwd);
            if (user != null)
            {
                if (ctx.Session["user"] == null)
                    ctx.Session.Add("user", user);
                else
                {
                    EntityLib.User preuser = ctx.Session["user"] as EntityLib.User;
                    if (preuser.UserID != user.UserID)
                        ctx.Session["user"] = user;
                }

                DateTime expire_time = ispersist ? DateTime.Now.AddMinutes(30) : DateTime.Now.AddYears(1);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    "LC@" + name, DateTime.Now, expire_time, ispersist, "member");
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));

                cookie.Expires = expire_time;
                ctx.Response.Cookies.Add(cookie);

                ctx.Session["cart"] = Cart.GetUserCart(user);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 用户直接登陆系统（已经是系统的用户）
        /// </summary>
        /// <param name="user"></param>
        public static void Login(CommonDevUser user)
        {
            if (user == null)
                return;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.Email,
                DateTime.Now, DateTime.Now.AddMinutes(30), false, user.UserID.ToString());
            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);
            HttpContext.Current.User = user;
        }
        #endregion

        public static void Logout()
        {
            CommonDevUser user = HttpContext.Current.User as CommonDevUser;
            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.Email,
                    DateTime.Now, DateTime.Now.AddDays(-1), false, user.UserID.ToString());
                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                HttpContext.Current.Response.Cookies.Add(authCookie);
                FormsAuthentication.SignOut();
            }
        }
    }
}
