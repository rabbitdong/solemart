﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using Com.Alipay;
using System.Configuration;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.WebUtil;
using Newtonsoft.Json;
using SimLogLib;

namespace Solemart.Web.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            SolemartUser user = this.User as SolemartUser;
            return View(user.Cart);
        }

        /// <summary>用户加入购物车操作
        /// </summary>
        /// <param name="id">加入购物车的物品ID</param>
        /// <returns>加入购物车后的View</returns>
        public ActionResult TotalAmount()
        {
            SolemartUser user = this.User as SolemartUser;

            //商品添加到购物车中（购物车显示商品的信息）
            decimal totalAmount = (int)user.Cart.CartItems.Sum(i => (i.Amount * i.UnitPrice));
            string resultData = JsonConvert.SerializeObject((new { TotalAmount = totalAmount.ToString("c") }));

            return Content((new WebResult<string> { ResultCode = WebResultCode.Success, ResultData = resultData }).ResponseString);
        }

        /// <summary>用户加入购物车操作
        /// </summary>
        /// <param name="id">加入购物车的物品ID</param>
        /// <returns>加入购物车后的View</returns>
        public ActionResult Add(int id)
        {
            SolemartUser user = this.User as SolemartUser;

            //商品添加到购物车中（购物车显示商品的信息）
            ProductItem product = ProductManager.GetProductByID(id);
            user.Cart.AddToCart(product, 1);
            decimal totalAmount = (int)user.Cart.CartItems.Sum(i => (i.Amount*i.UnitPrice));
            CartItem cartItem = user.Cart.CartItems.FirstOrDefault(i => i.ProductID == id);
            decimal theCount = 0;
            decimal theAmount = 0M;
            if (cartItem != null)
            {
                theCount = cartItem.Amount;
                theAmount = cartItem.Amount * cartItem.UnitPrice;
            }

            string theCountString = string.Empty;
            if (product.Unit == "斤")
                theCountString = theCount.ToString();
            else
                theCountString = string.Format("{0:d}", (int)theCount);

            string resultData = JsonConvert.SerializeObject((new {TotalAmount=totalAmount.ToString("c"), TheCount=theCountString, TheAmount = theAmount.ToString("c")}));

            return Content((new WebResult<string> { ResultCode = WebResultCode.Success, ResultData = resultData }).ResponseString);
        }

        /// <summary>用户修改购物车操作
        /// </summary>
        /// <param name="id">要修改的购物车的物品ID</param>
        /// <returns>修改后返回的View</returns>
        public ActionResult Modify(int id, decimal amount)
        {
            SolemartUser user = this.User as SolemartUser;
            if (user.Cart.ModifyCartItem(id, amount))
            {
                decimal totalAmount = (int)user.Cart.CartItems.Sum(i => (i.Amount * i.UnitPrice));
                ProductItem product = ProductManager.GetProductByID(id);

                decimal theCount = 0;
                decimal theAmount = 0M;
                if (amount > 0)
                {
                    CartItem cartItem = user.Cart.CartItems.FirstOrDefault(i => i.ProductID == id);
                    theCount = cartItem.Amount;
                    theAmount = cartItem.Amount * cartItem.UnitPrice;
                }
                string theCountString = string.Empty;
                if (product.Unit == "斤")
                    theCountString = theCount.ToString();
                else
                    theCountString = string.Format("{0:d}", (int)theCount);

                string resultData = JsonConvert.SerializeObject((new { TotalAmount = totalAmount.ToString("c"), TheCount = theCountString, TheAmount = theAmount.ToString("c") }));

                return Content((new WebResult<string> { ResultCode = WebResultCode.Success, ResultData = resultData }).ResponseString);
            }

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>用户请求进行结帐的处理
        /// </summary>
        /// <returns>用户进行结帐的视图</returns>
        public ActionResult CheckOut()
        {
            SolemartUser user = User as SolemartUser;
            if (user.Cart.CartItems == null || user.Cart.CartItems.Count == 0)
                return RedirectToAction("", "Home");

            return View(user.Cart);
        }

        /// <summary>用户请求保存送货信息的处理
        /// </summary>
        /// <returns>返回用户保存送货信息的结果View</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveAddrInfo(SendAddressItem addrInfo)
        {
            SolemartUser user = User as SolemartUser;
            addrInfo.DeliverWay = DeliverWay.ByManual;
            addrInfo.UserID = user.UserID;

            user.SendAddressInfo = addrInfo;

            return Content(WebResult<string>.SuccessResult.ResponseString);
        }

        /// <summary>用户提交订单的处理
        /// </summary>
        /// <returns>返回用户提交订单的结果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckoutOrder(SendAddressItem addrInfo, string remark)
        {
            SolemartUser user = User as SolemartUser;

            if(string.IsNullOrWhiteSpace(addrInfo.Address) || string.IsNullOrWhiteSpace(addrInfo.Phone))
                return Content(WebResult<string>.IncompleteInputResult.ResponseString);

            addrInfo.DeliverWay = DeliverWay.ByManual;
            addrInfo.UserID = user.UserID;

            user.SendAddressInfo = addrInfo;
            if (user.Cart.CartItems.Count == 0)
            {
                return Content(WebResult<string>.ParameterErrorResult.ResponseString);
            }

            OrderItem oi = new OrderItem();
            if (user.IsAnonymous)
                oi.UserID = SolemartUser.DefaultAnonymousUserID;
            else
                oi.UserID = user.UserID;

            oi.Receiver = addrInfo.Receiver;
            oi.Phone = addrInfo.Phone;
            oi.PaymentType = addrInfo.PaymentType;
            oi.PostCode = addrInfo.PostCode;
            oi.Address = addrInfo.Address;
            oi.DeliverWay = addrInfo.DeliverWay;
            oi.TotalPrice = user.Cart.TotalPrice;
            oi.Remark = remark;

            foreach (CartItem item in user.Cart.CartItems)
            {
                oi.OrderDetails.Add(new OrderDetailItem
                {
                    ProductID = item.ProductID,
                    Amount = item.Amount,
                    UnitPrice = item.UnitPrice
                });
            }

            if (!OrderManager.NewOrder(oi))
                return Content("error");

            //Clear the user's cart.
            //user.ClearCart();
            //新订单产生后，刷新最受欢迎的产品列表
            //ProductManager.RefleshMostPopularProducts();

            if (oi.OrderID > 0)
            {
                // 已进入订单后，临时购物车上物品需要清除
                user.Cart.ClearAndSave(user.UserID);

                //先保存用户的积分(目前的积分按购物的价格计算)。
                if (!user.IsAnonymous)
                {
                    user.AddPoint((int)oi.TotalPrice);
                }
#if false //支付宝暂时不支持
                if (oi.PaymentType == PaymentType.OnLine)
                {
                #region 填写支付宝参数
                    //支付类型
                    string payment_type = "1";
                    //必填，不能修改
                    //服务器异步通知页面路径
                    //string notify_url = "http://www.solemart.com/Cart/CheckOutComplete/"+orderid;
                    //需http://格式的完整路径，不能加?id=123这类自定义参数

                    //页面跳转同步通知页面路径
                    string return_url = ConfigurationManager.AppSettings["website"] + "Cart/CheckOutComplete/" + oi.OrderID;
                    //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

                    //卖家支付宝帐户
                    string seller_email = "18065911899";
                    //必填

                    //商户订单号
                    string out_trade_no = oi.OrderID.ToString("000000");
                    //商户网站订单系统中唯一订单号，必填

                    //订单名称
                    string subject = "叟玛特订单";
                    //必填

                    //付款金额
                    string price = oi.TotalPrice.ToString("#0.00");
                    //必填

                    //商品数量
                    string quantity = "1";
                    //必填，建议默认为1，不改变值，把一次交易看成是一次下订单而非购买一件商品
                    //物流费用
                    string logistics_fee = "0.00";
                    //必填，即运费
                    //物流类型
                    string logistics_type = "EXPRESS";
                    //必填，三个值可选：EXPRESS（快递）、POST（平邮）、EMS（EMS）
                    //物流支付方式
                    string logistics_payment = "SELLER_PAY";
                    //必填，两个值可选：SELLER_PAY（卖家承担运费）、BUYER_PAY（买家承担运费）
                    //订单描述

                    //string body = "";
                    //商品展示地址
                    //string show_url = "";
                    //需以http://开头的完整路径，如：http://www.Solemart.com/myorder.html

                    //收货人姓名
                    string receive_name = oi.Receiver;
                    //如：张三

                    //收货人地址
                    string receive_address = oi.Address;
                    //如：XX省XXX市XXX区XXX路XXX小区XXX栋XXX单元XXX号

                    //收货人邮编
                    string receive_zip = oi.PostCode == null ? "350600" : oi.PostCode;
                    //如：123456

                    //收货人电话号码
                    string receive_phone = oi.Phone;
                    //如：0571-88158090

                    //收货人手机号码
                    string receive_mobile = oi.Phone;
                    //如：13312341234


                    ////////////////////////////////////////////////////////////////////////////////////////////////

                    //把请求参数打包成数组
                    SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                    sParaTemp.Add("partner", Config.Partner);
                    sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
                    sParaTemp.Add("service", "trade_create_by_buyer");
                    sParaTemp.Add("payment_type", payment_type);
                    //sParaTemp.Add("notify_url", notify_url);
                    sParaTemp.Add("return_url", return_url);
                    sParaTemp.Add("seller_email", seller_email);
                    sParaTemp.Add("out_trade_no", out_trade_no);
                    sParaTemp.Add("subject", subject);
                    sParaTemp.Add("price", price);
                    sParaTemp.Add("quantity", quantity);
                    sParaTemp.Add("logistics_fee", logistics_fee);
                    sParaTemp.Add("logistics_type", logistics_type);
                    sParaTemp.Add("logistics_payment", logistics_payment);
                    //sParaTemp.Add("body", body);
                    sParaTemp.Add("receive_name", receive_name);
                    sParaTemp.Add("receive_address", receive_address);
                    sParaTemp.Add("receive_zip", receive_zip);
                    sParaTemp.Add("receive_phone", receive_phone);
                    sParaTemp.Add("receive_mobile", receive_mobile);
                    #endregion

                    //建立请求
                    string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                    return Content("ok-alipay-" + sHtmlText);
                }
                else
                {
                    return Content("ok-" + oi.OrderID);
                }
            }
            else
            {
#endif
                return Content(new WebResult<string> { ResultCode= WebResultCode.Success, ResultData = oi.OrderID.ToString() }.ResponseString);
            }
            else
                return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>用户确定结帐的处理
        /// </summary>
        /// <returns>返回结帐完成后的View</returns>
        public ActionResult CheckOutCompleted(int id)
        {
            int order_id = id;

            string trade_no = "";
            OrderItem current_order = null;
            SolemartUser current_user = User as SolemartUser;
            //支付宝付账后的转跳
            if (Request["trade_no"] != null && Request["is_success"] != null && Request["is_success"] == "T")
            {
                trade_no = Request["trade_no"];
                int.TryParse(Request["out_trade_no"].TrimStart('0'), out order_id);
                //判断是否付账成功
                if (IsPaySuccess())
                {
                    OrderManager.PayOrder(order_id, trade_no);
                }

                current_order = OrderManager.GetOrderInfo(order_id);
                if (!current_user.IsAnonymous || (current_user != null && current_order.User.UserName != current_user.UserName))
                {
                    return RedirectToAction("", "Home");
                }
            }
            else  //非支付宝提交后的转跳
            {
                current_order = OrderManager.GetOrderInfo(order_id);
            }

            return View(current_order);
        }

        /// <summary>用户取消订单的处理
        /// </summary>
        /// <param name="id">用户要取消的订单ID</param>
        /// <returns>用户取消订单的结果View</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CancelOrder(int id)
        {
            int order_id = id;

            SolemartUser user = User as SolemartUser;

            OrderItem order = OrderManager.GetOrderInfo(order_id);
            if (order.UserID != SolemartUser.DefaultAnonymousUserID && user.UserID != order.UserID)
                return Content(WebResult<string>.NormalErrorResult.ResponseString);

            if (OrderManager.CancelOrder(order_id))
            {
                //由于退货，扣除积分
                if (!user.IsAnonymous)
                {
                    user.TakeOffPoint((int)order.TotalPrice);
                }
                return Content(WebResult<string>.SuccessResult.ResponseString);
            }
            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        #region 支付宝处理
        /// <summary>获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>判断是否付账成功
        /// </summary>
        /// <returns></returns>
        private bool IsPaySuccess()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();
            Notify aliNotify = new Notify();
            bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);
            if (verifyResult)//验证成功
            {
                //交易状态
                string trade_status = Request.QueryString["trade_status"];

                if (Request.QueryString["trade_status"] == "WAIT_SELLER_SEND_GOODS")
                {
                    return true;
                }
                else if (Request.QueryString["trade_status"] == "TRADE_FINISHED")
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
