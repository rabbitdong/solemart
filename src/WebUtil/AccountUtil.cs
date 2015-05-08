using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using Solemart.BusinessLib;
using System.Threading;

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
                    "LC@" + email, DateTime.Now, DateTime.Now.AddMinutes(30), isPersist, user.UserID.ToString());
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                if(isPersist)
                    cookie.Expires = DateTime.Now.AddYears(1);
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
                HttpCookie authCookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
                if (authCookie != null)
                {
                    authCookie.Expires = DateTime.Now.AddYears(-1);
                    HttpContext.Current.Response.Cookies.Add(authCookie);
                }
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
            }
        }
    }

    /// <summary>
    /// User cache item.
    /// </summary>
    public class SolemartUserCacheItem
    {
        public SolemartUser SolemartUser { get; set; }
        public DateTime AddTime { get; set; }
    }

    /// <summary>
    /// 系统用户缓存类，它支持匿名用户。系统不应该直接使用SolemartUser建立用户等操作
    /// </summary>
    public class SolemartUserCache
    {
        //用于匿名用户的UserID
        private static int minAnonymousUserID = 200000000;
        private static int maxAnonymousUserID = int.MaxValue;

        private static List<SolemartUserCacheItem> _userCache = new List<SolemartUserCacheItem>();
        private static Timer _timer;

        static SolemartUserCache()
        {
            _timer = new Timer(ClearTimeoutUserCacheItem, null, 0, 30000);
        }

        private static int GetAnonymousUserID()
        {
            if (_userCache.Count == 0)
                return minAnonymousUserID;

            int maxUserID = _userCache.Select(u => u.SolemartUser.UserID).Max();
            if (maxUserID < minAnonymousUserID || maxUserID == maxAnonymousUserID)
                return minAnonymousUserID;

            return maxUserID + 1;
        }

        /// <summary>
        /// Get the user object from the cache.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static SolemartUser GetUser(int userid)
        {
            SolemartUserCacheItem item = _userCache.FirstOrDefault(u => u.SolemartUser.UserID == userid);
            if (item == null)
            {
                lock (_userCache)
                {
                    int anonymousUserID = GetAnonymousUserID();
                    item = new SolemartUserCacheItem { SolemartUser = new SolemartUser(userid, anonymousUserID), AddTime = DateTime.Now };
                    _userCache.Add(item);
                }
            }

            return item.SolemartUser;
        }

        /// <summary>
        /// Drop the user object in cache by manual.
        /// </summary>
        /// <param name="userid"></param>
        public static void DropUserInCache(int userid)
        {
            lock (_userCache)
            {
                SolemartUserCacheItem item = _userCache.FirstOrDefault(u => u.SolemartUser.UserID == userid);
                if (item != null)
                    _userCache.Remove(item);
            }
        }

        /// <summary>
        /// 清理超时的账号对象（超过20分钟的账号对象）
        /// </summary>
        /// <param name="obj">无用的参数</param>
        private static void ClearTimeoutUserCacheItem(object obj)
        {
            lock (_userCache)
            {
                //如果没有对象，就直接返回
                if (_userCache.Count == 0)
                    return;

                DateTime now = DateTime.Now;
                int count = _userCache.Count;
                for (int idx = count - 1; idx >= 0; --idx )
                {
                    if (_userCache[idx].AddTime < now.AddMinutes(-20))
                    {
                        _userCache.RemoveAt(idx);
                    }
                }
            }
        }

    }
}
