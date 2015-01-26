using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Xxx.WebUtil;
using Xxx.EntityLib;
using Xxx.BusinessLib;
using MyUser = Xxx.EntityLib.User;
using Xxx.SystemUtil;
using System.Collections.Specialized;
using Com.Alipay;
using System.Xml;
using System.Text.RegularExpressions;

namespace Xxx.Web.Controllers
{
    [Authorize(Roles = "su,operator,user")]
    public class AccountController : Controller
    {
        private OrderManager om = OrderManager.Instance;
        private UserManager um = UserManager.Instance;
        //
        // GET: /Account/

        public ActionResult Index()
        {
            MyUser user = Session["user"] as MyUser;
            if (user == null)
                return RedirectToAction("", "Home");

            return View(user);
        }

        /// <summary>用户修改注册信息的处理
        /// </summary>
        /// <param name="id">要修改的类型</param>
        /// <returns>修改结果的View</returns>
        public ActionResult Modify(string id) {
            MyUser CurrentUser = Session["user"] as MyUser;
            if (id == "uname") {
                return ModifyUserName();
            }

            if (id == "password") {
                return ModifyPassword();
            }

            string mod_val = Request["val"];
            if (mod_val == null || mod_val == "") {
                return Content("error");
            }

            DateTime birthday = DateTime.Now;
            if (id == "birthday") {
                if (DateTime.TryParse(mod_val, out birthday) && um.UpdateUserBirthDay(CurrentUser.UserID, birthday)) {
                    return Content("ok");
                }
                else {
                    return Content("error");
                }
            }

            if (id == "phone1") {
                if (!Regex.Match(mod_val, @"(^\d{8}$)|(^\d{11}$)").Success) {
                    return Content("error");
                }
            }

            if (um.UpdateUserInfo(CurrentUser.UserID, id, mod_val)) {
                if (id == "email")
                    CurrentUser.Email = mod_val;
                else
                    CurrentUser.AppendInfo = um.GetUserAppendInfo(CurrentUser.UserID);

                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>用户修改密码操作
        /// </summary>
        private ActionResult ModifyPassword() {
            MyUser CurrentUser = Session["user"] as MyUser;

            if (Request["newpwd"] == null || Request["newpwd"] == "") {
                return Content("error");
            }
            string oldpwd = Request["oldpwd"];
            string newpwd = Request["newpwd"];
            if (um.OnLogin(CurrentUser.Name, oldpwd) == null) {
                return Content("wrong pwd");
            }

            if (um.UpdateUserPwd(CurrentUser.UserID, newpwd)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>用于修改用户名(对于外部网站注册的用户)
        /// </summary>
        private ActionResult ModifyUserName() {
            MyUser CurrentUser = Session["user"] as MyUser;
            if (Request["val"] == null || Request["val"] == "") {
                return Content("error");
            }

            string nname = Request["val"];
            if (um.IsUserNameDuplicate(nname)) {
                return Content("error-duplicate");
            }

            if (!um.UpdateUserInfo(CurrentUser.UserID, "username", nname)) {
                return Content("error");
            }
            else {
                CurrentUser.Name = nname;
                return Content("ok");
            }
        }

        /// <summary>用户请求收藏页的处理
        /// </summary>
        /// <returns>用户收藏页面的视图</returns>
        public ActionResult Favorite() {
            MyUser user = Session["user"] as MyUser;
            IList<FavoriteInfo> MyFavorites = UserManager.Instance.GetUserFavoriteList(user.UserID);

            return View(MyFavorites);
        }

        /// <summary>用户把商品添加到收藏夹中的处理
        /// </summary>
        /// <param name="id">要进行收藏的商品ID</param>
        /// <returns>收藏结果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddToFovarite(int id) {
            MyUser user = Session["user"] as MyUser;
            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);
            if (user != null && user != MyUser.Anonymous && CurrentProduct != null) {
                if (UserManager.Instance.AddNewFavorite(user.UserID, CurrentProduct)) {
                    return Content("ok");
                }
            }

            return Content("error");
        }

        /// <summary>用户评论商品的处理
        /// </summary>
        /// <param name="id">要评论的商品的ID</param>
        /// <returns>评论结果</returns>
        public ActionResult MakeComment(int id) {
            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);
            string content = Request["cnt"];
            int level = 5;
            MyUser user = Session["user"] as MyUser;

            if (Request["level"] == null || !int.TryParse(Request["level"], out level) || CurrentProduct == null) {
                return Content("error");
            }

            if (ProductManager.Instance.CommentProduct(user, CurrentProduct, (EvaluteGrade)level, content)) {
                var comm_json = new {Name = user.Name, Time = DateTime.Now.ToLongDateString(), Level = level, Content = content };
                return Json(comm_json);
            }
            else {
                return Content("error");
            }
        }

        /// <summary>用户请求订单页的处理
        /// </summary>
        /// <returns>用户的订单页面视图</returns>
        public ActionResult Order() {
            MyUser user = Session["user"] as MyUser;
            OrderManager om = OrderManager.Instance;

            int TotalOrderCount = om.GetUserOrderCount(user, OrderStatus.AllStatus);
            ViewData["PageCount"] = (TotalOrderCount + om.PageSize - 1) / om.PageSize;

            int page_index = 0;
            List<OrderItem> MyOrders = null;

            if (Request["p"] != null && int.TryParse(Request["p"], out page_index)) {
                MyOrders = om.GetUserPagedOrder(user, page_index - 1);
                ViewData["CurrentPageIndex"] = page_index;
            }
            else {
                MyOrders = om.GetUserPagedOrder(user, 0);
                ViewData["CurrentPageIndex"] = 1;
            }

            return View(MyOrders);
        }

        /// <summary>用户获取订单信息的处理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OrderDetail(int id) {
            int order_id = id;

            OrderItem oi = om.GetOrderInfo(order_id);
            if (oi == null) {
                return Content("error");
            }

            IEnumerable<ProductItem> products = oi.Products;
            var items = from p in products
                        select new {
                            ProductID = p.Product.ProductID,
                            Name = p.Product.Name,
                            Amount = p.Amount,
                            UnitPrice = p.UnitPrice
                        };

            return Json(products);
        }

        /// <summary>用户取消订单的处理
        /// </summary>
        /// <param name="id">用户要取消的订单ID</param>
        /// <returns>用户取消订单的结果View</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CancelOrder(int id) {
            int order_id = id;            

            if (om.CancelOrder(order_id)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>用户确认订单的操作处理
        /// </summary>
        /// <param name="id">用户要确认的订单ID</param>
        /// <returns>用户确认订单的结果</returns>
        public ActionResult ConfirmOrder(int id) {
            int order_id = id;

            OrderItem oi = om.GetOrderInfo(order_id);
            if (oi.AddressInfo.Pay == PayType.OnLine) {
                ComfirmAlipay(order_id);
            }

            if (om.ConfirmOrder(order_id)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        private bool ComfirmAlipay(int order_id) {
            OrderItem oi = om.GetOrderInfo(order_id);
            if (oi == null)
                return false;

            //支付宝交易号
            string trade_no = oi.TradeNo;
            //必填
            //物流公司名称
            string logistics_name = "其它";
            //必填
            //物流发货单号
            string invoice_no = order_id.ToString("000000");
            //物流运输类型
            string transport_type = "EXPRESS";
            //三个值可选：POST（平邮）、EXPRESS（快递）、EMS（EMS）

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "send_goods_confirm_by_platform");
            sParaTemp.Add("trade_no", trade_no);
            sParaTemp.Add("logistics_name", logistics_name);
            sParaTemp.Add("invoice_no", invoice_no);
            sParaTemp.Add("transport_type", transport_type);

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp);

            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.LoadXml(sHtmlText);
                string strXmlResponse = xmlDoc.SelectSingleNode("/alipay/is_success").InnerText;
                LogManager.Instance.Error("alipay - " + strXmlResponse);
                if (strXmlResponse == "T") {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception exp) {
                return false;
            }
        }
    }
}
