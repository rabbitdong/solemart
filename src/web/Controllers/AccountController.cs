using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Solemart.WebUtil;
using System.Collections.Specialized;
using System.Xml;
using System.Text.RegularExpressions;
using Com.Alipay;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.Web.Models;


namespace Solemart.Web.Controllers
{
    [Authorize(Roles = "Super,Operator,NormalUser")]
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            if (User == null)
                return RedirectToAction("", "Home");

            return View(User as SolemartUser);
        }

        /// <summary>
        /// Modify the user's information
        /// </summary>
        /// <param name="userinfo">要修改的类型</param>
        /// <returns>修改结果的View</returns>
        public ActionResult Modify(UserItem userinfo)
        {
            return Content("ok");
        }

        /// <summary>用户修改密码操作
        /// </summary>
        private ActionResult ModifyPassword()
        {
            return Content("ok");
        }

        /// <summary>用于修改用户名(对于外部网站注册的用户)
        /// </summary>
        private ActionResult ModifyUserName()
        {
            //MyUser CurrentUser = Session["user"] as MyUser;
            //if (Request["val"] == null || Request["val"] == "")
            //{
            //    return Content("error");
            //}

            //string nname = Request["val"];
            //if (um.IsUserNameDuplicate(nname))
            //{
            //    return Content("error-duplicate");
            //}

            //if (!um.UpdateUserInfo(CurrentUser.UserID, "username", nname))
            //{
            //    return Content("error");
            //}
            //else
            //{
            //    CurrentUser.Name = nname;
            //    return Content("ok");
            //}
            return Content("ok");
        }

        /// <summary>用户请求收藏页的处理
        /// </summary>
        /// <returns>用户收藏页面的视图</returns>
        public ActionResult Favorite()
        {
            SolemartUser user = User as SolemartUser;
            //IList<FavoriteItem> MyFavorites = UserManager.GetUserFavoriteList(user.UserID);

            return View("");
        }

        /// <summary>
        /// The user add the product to favorite list
        /// </summary>
        /// <param name="productID">要进行收藏的商品ID</param>
        /// <returns>收藏结果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddToFovarite(int productID)
        {
            SolemartUser user = User as SolemartUser;
            SaledProductItem CurrentProduct = ProductManager.GetSaledProductByID(productID);
            if (user != null && !user.IsAnonymous && CurrentProduct != null)
            {
                if (UserManager.AddNewFavorite(user.UserID, CurrentProduct.ProductID))
                {
                    return Content("ok");
                }
            }

            return Content("error");
        }

        /// <summary>
        /// The user comment the product
        /// </summary>
        /// <param name="id">要评论的商品的ID</param>
        /// <returns>评论结果</returns>
        public ActionResult MakeComment(int id)
        {
            SaledProductItem CurrentProduct = ProductManager.GetSaledProductByID(id);
            string content = Request["cnt"];
            int level = 5;

            if (Request["level"] == null || !int.TryParse(Request["level"], out level) || CurrentProduct == null)
            {
                return Content("error");
            }

            SolemartUser user = User as SolemartUser;
            if (ProductManager.CommentProduct(user.UserID, CurrentProduct.ProductID, (EvaluteGrade)level, content))
            {
                var comm_json = new { Name = user.UserName, Time = DateTime.Now.ToLongDateString(), Level = level, Content = content };
                return Json(comm_json);
            }
            else
            {
                return Content("error");
            }
        }

        /// <summary>
        /// The user view the orders
        /// </summary>
        /// <returns>用户的订单页面视图</returns>
        public ActionResult Order(int? p)
        {
            SolemartUser user = User as SolemartUser;

            int pageIndex = p ?? 0;
            int totalCount = 0;
            List<OrderItem> MyOrders = OrderManager.GetPagedUserOrder(user.UserID, pageIndex, 10, out totalCount);

            OrderListViewModel model = new OrderListViewModel();
            model.PageIndex = pageIndex;
            model.TotalPageCount = (totalCount + 9) / 10;
            model.OrderList = MyOrders;

            return View(model);
        }

        /// <summary>用户获取订单信息的处理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OrderDetail(int id)
        {
            OrderItem oi = OrderManager.GetOrderInfo(id);
            if (oi == null)
            {
                return Content("error");
            }

            IEnumerable<OrderDetailItem> products = oi.OrderDetails;
            var items = from p in products
                        select new
                        {
                            ProductID = p.Product.ProductID,
                            Name = p.Product.ProductName,
                            Amount = p.Amount,
                            UnitPrice = p.UnitPrice
                        };

            return Json(items);
        }

        /// <summary>用户确认订单的操作处理
        /// </summary>
        /// <param name="id">用户要确认的订单ID</param>
        /// <returns>用户确认订单的结果</returns>
        public ActionResult ConfirmOrder(int id)
        {
            int order_id = id;

            OrderItem oi = OrderManager.GetOrderInfo(order_id);
            if (oi.PaymentType == PaymentType.OnLine)
            {
                ComfirmAlipay(order_id);
            }

            if (OrderManager.ConfirmOrder(order_id))
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        private bool ComfirmAlipay(int order_id)
        {
            OrderItem oi = OrderManager.GetOrderInfo(order_id);
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
            try
            {
                xmlDoc.LoadXml(sHtmlText);
                string strXmlResponse = xmlDoc.SelectSingleNode("/alipay/is_success").InnerText;
                if (strXmlResponse == "T")
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
