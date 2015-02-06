using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.EntityLib;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Com.Alipay;
using System.Xml;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "su,operator")]
    public class OrdersController : Controller
    {
        OrderManager om = OrderManager.Instance;
        //
        // GET: /Manager/Orders/
        public ActionResult Index(int? id) {
            int pi=id??1;  //表示页索引

            ViewData["PageCount"] = (om.GetNewOrderCount() + 9) / 10;
            ViewData["CurrentPageIndex"] = pi;

            List<OrderItem> NewOrders = OrderManager.Instance.GetPagedOrders(OrderStatus.Ordered, pi - 1);
            return View(NewOrders);
        }

        /// <summary>获取用户的新订单的请求处理
        /// </summary>
        /// <returns>返回用户获取新订单的结果View</returns>
        public ActionResult New(int? id)
        {
            int pi = id??1; //表示页索引

            ViewData["PageCount"] = (om.GetNewOrderCount() + 9) / 10;
            ViewData["CurrentPageIndex"] = pi;

            List<OrderItem> NewOrders = OrderManager.Instance.GetPagedOrders(OrderStatus.Ordered, pi - 1);

            return View("Index", NewOrders);
        }

        /// <summary>获取在发送中的订单的请求处理
        /// </summary>
        /// <returns>返回在发送中的订单的结果View</returns>
        public ActionResult Sending() {
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            ViewData["PageCount"] = 1;
            ViewData["CurrentPageIndex"] = pi;

            List<OrderItem> SendingOrders = OrderManager.Instance.GetPagedOrders(OrderStatus.Sending, pi - 1);
            return View("Index", SendingOrders);
        }

        /// <summary>获取已经接收订单的列表请求处理
        /// </summary>
        /// <returns>返回已经接收的订单的请求结果View</returns>
        public ActionResult Received() {
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            ViewData["PageCount"] = 1;
            ViewData["CurrentPageIndex"] = pi;

            List<OrderItem> ReceivedOrders = om.GetPagedOrders(OrderStatus.Received, pi - 1);
            return View("Index", ReceivedOrders);
        }

        /// <summary>管理员发送订单请求的处理
        /// </summary>
        /// <param name="id">要发送的订单ID</param>
        /// <returns>返回发送请求的结果View</returns>
        public ActionResult SendOrder(int id) {
            OrderItem CurrentOrder = om.GetOrderInfo(id);
            if (CurrentOrder == null) {
                return Content("Order ID is not exist!");
            }

            return View(CurrentOrder);
        }

        /// <summary>操作员发货请求的处理
        /// </summary>
        /// <param name="id">要发货的订单ID</param>
        /// <returns>返回发货处理的结果View</returns>
        public ActionResult Send(int id) {
            OrderItem oi = om.GetOrderInfo(id);
            if (oi == null)
                return Content("error-order no found!");
            if (oi.AddressInfo.Pay == PaymentType.OnLine) {
                LogManager.Instance.Log(string.Format("确认发货，订单号：{0}，结果:{1}", id, ComfirmAlipay(oi)));
            }
            om.SendOrder(id);
            ProductManager.Instance.ShippingProducts(oi.Products);

            return Content("ok");
        }

        /// <summary>支付宝确认发货
        /// </summary>
        /// <param name="oi"></param>
        /// <returns></returns>
        private bool ComfirmAlipay(OrderItem oi) {
            if (oi == null)
                return false;

            //支付宝交易号
            string trade_no = oi.TradeNo;
            //必填
            //物流公司名称
            string logistics_name = "其它";
            //必填
            //物流发货单号
            string invoice_no = oi.OrderID.ToString("000000");
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
