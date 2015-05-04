using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Com.Alipay;
using Solemart.Web.Areas.Manager.Models;
using Solemart.WebUtil;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Super,Operator")]
    public class OrdersController : Controller
    {
        public ActionResult Index(int? id)
        {
            int pi = id ?? 0;  //表示页索引
            int totalCount = 0;

            List<OrderItem> orderList = OrderManager.GetPagedOrders(OrderStatus.Ordered, pi, 10, out totalCount);
            OrderManagerViewModel model = new OrderManagerViewModel();

            model.OrderList = orderList;
            model.PageIndex = pi;
            model.TotalPageCount = (totalCount + 9) / 10;

            return View(model);
        }

        /// <summary>
        /// Get the paged new orders
        /// </summary>
        /// <returns>返回用户获取新订单的结果View</returns>
        public ActionResult New(int? id)
        {
            int pi = id ?? 0; //表示页索引
            int totalCount = 0;

            List<OrderItem> orderList = OrderManager.GetPagedOrders(OrderStatus.Ordered, pi, 10, out totalCount);
            OrderManagerViewModel model = new OrderManagerViewModel();

            model.OrderList = orderList;
            model.PageIndex = pi;
            model.TotalPageCount = (totalCount + 9) / 10;
            return View("Index", model);
        }

        /// <summary>获取在发送中的订单的请求处理
        /// </summary>
        /// <returns>返回在发送中的订单的结果View</returns>
        public ActionResult Sending(int? p)
        {
            int pi = p ?? 0; //表示页索引
            int totalCount = 0; 

            List<OrderItem> orderList = OrderManager.GetPagedOrders(OrderStatus.Sending, pi, 10, out totalCount);
            OrderManagerViewModel model = new OrderManagerViewModel();

            model.OrderList = orderList;
            model.PageIndex = pi;
            model.TotalPageCount = (totalCount + 9) / 10;

            return View("Index", model);
        }

        /// <summary>获取已经接收订单的列表请求处理
        /// </summary>
        /// <returns>返回已经接收的订单的请求结果View</returns>
        public ActionResult Received(int? p)
        {
            int pi = p ?? 0; //表示页索引
            int totalCount = 0;

            List<OrderItem> orderList = OrderManager.GetPagedOrders(OrderStatus.Received, pi, 10, out totalCount);
            OrderManagerViewModel model = new OrderManagerViewModel();

            model.OrderList = orderList;
            model.PageIndex = pi;
            model.TotalPageCount = (totalCount + 9) / 10;

            return View("Index", model);
        }

        /// <summary>管理员发送订单请求的处理
        /// </summary>
        /// <param name="id">要发送的订单ID</param>
        /// <returns>返回发送请求的结果View</returns>
        public ActionResult SendOrder(int id)
        {
            OrderItem CurrentOrder = OrderManager.GetOrderInfo(id);
            if (CurrentOrder == null)
            {
                return Content("Order ID is not exist!");
            }

            return View(CurrentOrder);
        }

        /// <summary>操作员发货请求的处理
        /// </summary>
        /// <param name="id">要发货的订单ID</param>
        /// <returns>返回发货处理的结果View</returns>
        public ActionResult Send(int id)
        {
            OrderItem oi = OrderManager.GetOrderInfo(id);
            if (oi == null)
                return Content("error-order no found!");
            if (oi.PaymentType == PaymentType.OnLine)
            {
                ComfirmAlipay(oi);
            }
            OrderManager.SendOrder(id);
            ProductManager.ShippingProducts(oi.OrderDetails);

            return Content("ok");
        }

        /// <summary>支付宝确认发货
        /// </summary>
        /// <param name="oi"></param>
        /// <returns></returns>
        private bool ComfirmAlipay(OrderItem oi)
        {
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
