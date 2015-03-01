using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using Solemart.BusinessLib;

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
        public static bool Login(string email, string pwd, bool isPersist)
        {
            HttpContext ctx = HttpContext.Current;
            SolemartUser user = UserManager.OnLogin(email, pwd);
            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    "LC@" + email, DateTime.Now, DateTime.Now.AddMinutes(30), isPersist, "member");
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                if(isPersist)
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                ctx.User = user;
                ctx.Response.Cookies.Add(cookie);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 用户直接登陆系统（已经是系统的用户）
        /// </summary>
        /// <param name="user"></param>
        public static void Login(SolemartUser user)
        {
            if (user == null)
                return;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserName,
                DateTime.Now, DateTime.Now.AddMinutes(30), false, user.UserID.ToString());
            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);
            HttpContext.Current.User = user;
        }
        #endregion

        public static void Logout()
        {
            SolemartUser user = HttpContext.Current.User as SolemartUser;
            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserName,
                    DateTime.Now, DateTime.Now.AddDays(-1), false, user.UserID.ToString());
                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                authCookie.Expires = DateTime.Now;
                HttpContext.Current.Response.Cookies.Add(authCookie);
                FormsAuthentication.SignOut();
            }
        }
    }
}
