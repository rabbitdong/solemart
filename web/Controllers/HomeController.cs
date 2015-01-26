﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xxx.EntityLib;
using Xxx.BusinessLib;
using SolemartUser = Xxx.EntityLib.User;
using System.Web.Security;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Xxx.Web.Controllers {
    public class HomeController : Controller {
        /// <summary>在UI上显示的正常的商品列表
        /// </summary>
        protected IList<Product> ProductList = null;

        private CategoryManager cm = CategoryManager.Instance;
        private ProductManager pm = ProductManager.Instance;

        public ActionResult Index() {
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            ProductList = pm.GetSalePagedProducts(pi - 1);    //处理的页索引从0开始，这个需要减1
            ViewData["CurrentPageIndex"] = pi;
            ViewData["PageCount"] = pm.SaleProductPagedCount;
            
            return View(ProductList);
        }

        /// <summary>用户注册的处理
        /// </summary>
        /// <returns>返回用户注册页面</returns>
        public ActionResult Register() {
            return View();
        }

        /// <summary>用于验证微信公众号的信息
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateWeixin() {
            string signature = Request["signature"];
            string timestamp = Request["timestamp"];
            string nonce = Request["nonce"];
            string token = "solemart";   
     		string[] tmpArr = {token, timestamp, nonce};
            Array.Sort(tmpArr);
            string tmpStr = tmpArr[0] + tmpArr[1] + tmpArr[2];
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
	
            string echostr = Request["echostr"];
            return Content(echostr);
        }

        /// <summary>用户注销操作
        /// </summary>
        /// <returns>返回注销结果</returns>
        public ActionResult Logout() {
            SolemartUser user = Session["user"] as SolemartUser;
            if (user == null || user == SolemartUser.Anonymous)
                return Content("ok");

            Cart cart = Session["cart"] as Cart;
            if (cart.Products.Count > 0)
                cart.Save(Session["user"] as User);

            this.Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            Response.Cookies.Add(cookie);

            HttpContext.User = new System.Security.Principal.GenericPrincipal(
              new System.Security.Principal.GenericIdentity(EntityLib.User.Anonymous.Name),
              EntityLib.Role.GetRoleNames(EntityLib.User.Anonymous.Roles));

            return Content("ok");
        }

        /// <summary>用户注册一个新帐号的处理
        /// </summary>
        /// <returns>注册新帐号的处理结果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegisterNew() {
            UserManager um = UserManager.Instance;

            string uname = Request["name"];
            string pwd = Request["pwd"];
            string email = Request["email"];
            bool is_subscript = Request["subscript"] != null && Request["subscript"] == "true";

            if (um.IsUserNameDuplicate(uname)) {
                return Content("error-name duplicate");
            }

            if (um.IsEmailDuplicate(email)) {
                return Content("error-email duplicate");
            }

            SolemartUser user = um.AddNewUser(uname, pwd, email);
            if (user != null) {
                Xxx.WebUtil.Util.UserLogin(uname, pwd, true);
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>用户输入关键字搜索产品列表
        /// </summary>
        /// <param name="id">用户输入的关键字</param>
        /// <returns>返回的产品列表</returns>
        public ActionResult Search(string id) {
            ProductList = Searcher.Instane.GetPagedProductsByKey(id);
            ViewData["PageCount"] = 0;
            ViewData["CurrentPageIndex"] = 1;
            return View("Index", ProductList);
        }

        /// <summary>返回登录的View
        /// </summary>
        /// <returns>返回登录的View</returns>
        public ActionResult Login() {
            return View();
        }

        /// <summary>用户进行登录的处理
        /// </summary>
        /// <returns>返回用户登录的结果</returns>
        public ActionResult OnLogin() {
            string name = Request["uname"];
            string pwd = Request["pwd"];
            bool ispersist = false;

            if (Request["ispersist"] == "true")
                ispersist = true;

            if (Xxx.WebUtil.Util.UserLogin(name, pwd, ispersist)) {
                string redirect_url = FormsAuthentication.GetRedirectUrl("LC@" + name, ispersist);
                if (redirect_url == null || redirect_url == "")
                    redirect_url = "/";

                return Content(redirect_url);
            }
            else
                return Content("error");
        }

        /// <summary>
        /// </summary>
        public ActionResult OnQQLogin() {
            string nickname = Request["nickname"];
            string openid = Request["openid"];

            if (openid == null || openid == "")
                return Content("error");

            User user = UserManager.Instance.OnQQLogin(openid, nickname);
            if (user != null) {
                if (Session["user"] == null)
                    Session.Add("user", user);
                else {
                    SolemartUser preuser = Session["user"] as SolemartUser;
                    if (preuser.UserID != user.UserID)
                        Session["user"] = user;
                }

                DateTime expire_time = DateTime.Now.AddYears(1);

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    "QQ&" + openid, DateTime.Now, expire_time, true, "member");
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));

                cookie.Expires = expire_time;
                Response.Cookies.Add(cookie);

                Session["cart"] = Cart.GetUserCart(user);
            }

            if (user != null) {
                string redirect_url = FormsAuthentication.GetRedirectUrl("QQ@" + openid, true);
                if (redirect_url == null || redirect_url == "")
                    redirect_url = "/";

                return Content(redirect_url);
            }
            else {
                return Content("error");
            }
        }

        /// <summary>用户取回密码的处理
        /// </summary>
        /// <returns>返回取回密码页面</returns>
        public ActionResult GetPwd() {
            return View();
        }

        /// <summary>进行用户重置密码操作的处理
        /// </summary>
        /// <returns>返回重置密码操作的结果</returns>
        public ActionResult ResetPwd() {
            string email = Request["email"];
            string from = "adon_hua@hotmail.com";

            string pwd = UserManager.Instance.GenerateRandomPwd();

            SmtpClient client = new SmtpClient("...");
            client.Credentials = new NetworkCredential(from, "...");
            client.EnableSsl = true;

            string content = string.Format("您的新密码是:{0}, 请登录网站{1}修改你的密码", pwd, "http://xxx/");
            client.Send(from, email, "您的密码重置密码----来自XXX.com的邮件", pwd);
            return Content("邮件已经发送，请查收");
        }

        /// <summary>判断当前用户是否是注册用户的处理
        /// </summary>
        /// <returns>判断当前用户是否是注册用户的结果：ok - 是注册用户, error - 是匿名用户</returns>
        public ActionResult IsAuthenticate() {
            SolemartUser user = Session["user"] as SolemartUser;
            if (user != null && user != SolemartUser.Anonymous)
                return Content("ok");
            else
                return Content("error");
        }
    }
}
