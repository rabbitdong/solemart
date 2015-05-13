using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.WebUtil;
using Solemart.Web.Models;
using SimLogLib;
using LogUtil = SimLogLib.Util;

namespace Solemart.Web.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Display the product list.
        /// </summary>
        public ActionResult Index(int? p)
        {
            int pi = p ?? 0; //表示页索引
            int totalPageCount = 0;
            List<SaledProductItem> products = ProductManager.GetPagedSaledProducts(pi, 50, out totalPageCount);

            ProductListViewModel model = new ProductListViewModel();
            model.PageIndex = pi;
            model.TotalPageCount = totalPageCount;
            foreach (SaledProductItem product in products)
            {
                ProductForListViewModel pmodel = new ProductForListViewModel();
                ProductItem productItem = product.Product;
                ProductImageItem imageItem = ProductManager.GetProductLogoImage(product.ProductID);
                pmodel.ProductID = product.ProductID;
                pmodel.ProductName = product.Product.ProductName;
                pmodel.Price = product.Price;
                pmodel.Discount = product.Discount;
                pmodel.IsSpecial = product.SpecialFlag;
                pmodel.Unit = product.Product.Unit;
                pmodel.IsOutOfStock = (product.Product.StockCount - product.Product.ReserveCount) == 0;
                pmodel.ProductImageName = productItem.ProductName;
                if (imageItem != null)
                    pmodel.ProductImageUrl = imageItem.ImageUrl;
                else
                    pmodel.ProductImageUrl = "no-img.png";
                model.ProductList.Add(pmodel);
            }

            return View(model);
        }

        /// <summary>
        /// <summary>用户注册的处理
        /// </summary>
        /// <returns>返回用户注册页面</returns>
        public ActionResult Register()
        {
            return View();
        }



        /// <summary>
        /// The interface for validating from the weixin
        /// </summary>
        /// <returns></returns>
        /// signature  微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。  
        /// timestamp  时间戳  
        /// nonce  随机数  
        /// echostr  随机字符串  
        public ActionResult ValidateWeixin(string signature, string timestamp, string nonce, string echostr)
        {
            string token = "solemart";
            string[] tmpArr = { token, timestamp, nonce };
            Array.Sort(tmpArr);
            string tmpStr = tmpArr[0] + tmpArr[1] + tmpArr[2];
            string resultStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            Log.Instance.WriteLog(string.Format("ValidateWeixin: request[{0}], calculate[{1}]", tmpStr, resultStr));

            return Content(echostr);
        }

        /// <summary>用户注销操作
        /// </summary>
        /// <returns>返回注销结果</returns>
        public ActionResult Logout()
        {
            AccountUtil.Logout();
            return Content(WebResult<string>.SuccessResult.ResponseString);
        }

        public ActionResult GetLog()
        {
            List<LogItem> logs = Log.Instance.GetLog(DateTime.Now);
            StringBuilder sb = new StringBuilder();
            foreach (LogItem log in logs)
            {
                sb.AppendFormat("{0}: {1}<br/>", log.Time, log.Message);
            }

            return Content(sb.ToString());
        }

        /// <summary>用户注册一个新帐号的处理
        /// </summary>
        /// <returns>注册新帐号的处理结果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegisterNew(string userName, string password, string email)
        {
            if (UserManager.IsUserNameDuplicate(userName))
            {
                return Content(new WebResult<string> { ResultCode = WebResultCode.DuplicatedField, ResultData="username", ResultMessage = "用户名已经被使用" }.ResponseString);
            }

            if (UserManager.IsEmailDuplicate(email))
            {
                return Content(new WebResult<string> { ResultCode = WebResultCode.DuplicatedField, ResultData="email", ResultMessage = "Email已经被使用" }.ResponseString);
            }

            UserItem user = UserManager.AddNewUser(userName, password, email);
            if (user != null)
            {
                Solemart.WebUtil.AccountUtil.Login(user.UserName, password, false);
                return Content(WebResult<string>.SuccessResult.ResponseString);
            }
            else
            {
                return Content(WebResult<string>.NormalErrorResult.ResponseString);
            }
        }

        /// <summary>用户输入关键字搜索产品列表
        /// </summary>
        /// <param name="id">用户输入的关键字</param>
        /// <returns>返回的产品列表</returns>
        public ActionResult Search(string id)
        {
            return View("Index");
        }

        /// <summary>返回登录的View
        /// </summary>
        /// <returns>返回登录的View</returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>用户进行登录的处理
        /// </summary>
        /// <returns>返回用户登录的结果</returns>
        public ActionResult OnLogin(string username, string password, bool isPersist, string returnUrl)
        {
            if (Solemart.WebUtil.AccountUtil.Login(username, password, isPersist))
            {
                string redirect_url = FormsAuthentication.GetRedirectUrl("LC@" + username, isPersist);
                if (redirect_url == null || redirect_url == "")
                    redirect_url = "/";

                returnUrl = string.Format("/{0}", returnUrl);
                return Content(new WebResult<string> { ResultCode = WebResultCode.Success, ResultData = returnUrl }.ResponseString);
            }
            else
                return Content(WebResult<string>.UserNameOrPasswordErrorResult.ResponseString);
        }

        /// <summary>
        /// </summary>
        public ActionResult OnQQLogin()
        {
            string nickname = Request["nickname"];
            string openid = Request["openid"];

            if (openid == null || openid == "")
                return Content("error");

            UserItem user = UserManager.OnQQLogin(openid, nickname);
            if (user != null)
            {
                if (Session["user"] == null)
                    Session.Add("user", user);
                else
                {
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
            }

            if (user != null)
            {
                string redirect_url = FormsAuthentication.GetRedirectUrl("QQ@" + openid, true);
                if (redirect_url == null || redirect_url == "")
                    redirect_url = "/";

                return Content(redirect_url);
            }
            else
            {
                return Content("error");
            }
        }

        /// <summary>用户取回密码的处理
        /// </summary>
        /// <returns>返回取回密码页面</returns>
        public ActionResult GetPwd()
        {
            return View();
        }

        /// <summary>进行用户重置密码操作的处理
        /// </summary>
        /// <returns>返回重置密码操作的结果</returns>
        public ActionResult ResetPwd()
        {
            string email = Request["email"];
            string pwd = EncryptUtil.GenerateRandomPassword();

            string content = string.Format("您的新密码是:{0}, 请登录网站{1}修改你的密码", pwd, "http://xxx/");
            EmailServer.SendEmail(email, "", content);
            return Content("邮件已经发送，请查收");
        }

        /// <summary>判断当前用户是否是注册用户的处理
        /// </summary>
        /// <returns>判断当前用户是否是注册用户的结果：ok - 是注册用户, error - 是匿名用户</returns>
        public ActionResult IsAuthenticate()
        {
            SolemartUser user = Session["user"] as SolemartUser;
            if (user != null && !user.IsAnonymous)
                return Content("ok");
            else
                return Content("error");
        }
    }
}
