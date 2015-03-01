using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;
using Solemart.SystemUtil;

namespace Solemart.BusinessLib {
    /// <summary>订单处理类
    /// </summary>
    public class OrderManager 
    {
        /// <summary>
        /// Generate a new order
        /// </summary>
        /// <param name="orderItem">该订单的产品列表</param>
        /// <returns>return true if success, or false</returns>
        public static bool NewOrder(OrderItem orderItem) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                context.OrderItems.Add(orderItem);

                foreach (OrderDetailItem odi in orderItem.OrderDetails)
                {
                    context.OrderDetailItems.Add(odi);
                }

                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Get the order info
        /// </summary>
        /// <param name="orderID">某个订单的订单ID</param>
        /// <returns>如果获取订单，返回该订单对象，否则返回null</returns>
        public static OrderItem GetOrderInfo(int orderID) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.OrderItems.Include("OrderDetails").FirstOrDefault(o => o.OrderID == orderID);
            }
        }

        /// <summary>
        /// Get the paged orders of the user
        /// </summary>
        /// <param name="userID">要获取订单的用户</param>
        /// <param name="page_size">分页的大小，页索引从0开始</param>
        /// <returns>订单列表</returns>
        public static List<OrderItem> GetPagedUserOrder(int userID, int pageIndex, int pageSize, out int totalPageCount) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from o in context.OrderItems
                        orderby o.OrderTime descending
                        where o.UserID == userID
                        select o;
                totalPageCount = (q.Count() - 1) / pageSize + 1;
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Get the orders count
        /// </summary>
        /// <param name="orderStatus">订单的类别</param>
        /// <returns>获取到的订单总数</returns>
        public static int GetOrderCount(OrderStatus orderStatus) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.OrderItems.Count(o => (o.Status == orderStatus));
            }
        }

        /// <summary>
        /// Get the orders count of the user
        /// </summary>
        /// <param name="userID">获取的用户</param>
        /// <param name="status">获取的订单的状态</param>
        /// <returns>获取到的订单的数量</returns>
        /// <remarks>如果status值是AllStatus，获取该用户的所有订单的数量</remarks>
        public static int GetUserOrderCount(int userID, OrderStatus orderStatus)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                if (orderStatus == OrderStatus.AllStatus)
                    return context.OrderItems.Count(o => (o.UserID == userID));
                else
                    return context.OrderItems.Count(o => (o.UserID == userID && o.Status == orderStatus));
            }
        }

        /// <summary>
        /// Get the paged order list
        /// </summary>
        /// <param name="pageIndex">获取订单的页索引(从0开始)</param>
        /// <param name="pageSize">分页的大小</param>
        /// <returns>获取到的订单列表(按时间倒序排列)</returns>
        public static List<OrderItem> GetPagedOrders(OrderStatus orderStatus, int pageIndex, int pageSize, out int totalPageCount) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from o in context.OrderItems.Include("OrderDetails")
                        orderby o.OrderTime descending
                        where o.Status == orderStatus
                        select o;
                totalPageCount = (q.Count() - 1) / pageSize + 1;
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Sending the order
        /// </summary>
        /// <param name="orderID">要发货的订单的订单号</param>
        /// <returns>是否发货成功</returns>
        public static bool SendOrder(int orderID) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                OrderItem order = context.OrderItems.Find(orderID);
                if (order == null || order.Status != OrderStatus.Ordered)
                    return false;
                order.Status = OrderStatus.Sending;
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// User pay the order
        /// </summary>
        /// <param name="orderID">要支付的订单的ID</param>
        /// <returns>是否成功支付</returns>
        public static bool PayOrder(int orderID, string tradeNo = "") 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                OrderItem order = context.OrderItems.Find(orderID);
                if (order == null || order.HasPay)
                    return false;

                order.HasPay = true;
                order.TradeNo = tradeNo;
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// The user confirm the order
        /// </summary>
        /// <param name="orderID">要确认的订单ID</param>
        /// <returns>用户的确认结果</returns>
        public static bool ConfirmOrder(int orderID) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                OrderItem order = context.OrderItems.Find(orderID);
                if (order == null || order.Status != OrderStatus.Sending)
                    return false;
                order.Status = OrderStatus.Received;
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Get the new order count
        /// </summary>
        public static int GetNewOrderCount()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.OrderItems.Count(o => (o.Status == OrderStatus.Ordered));
            }
        }

        /// <summary>
        /// The user cancel the order
        /// </summary>
        /// <param name="orderID">要取消的订单ID</param>
        /// <returns>是否取消成功</returns>
        public static bool CancelOrder(int orderID) 
        {
            return true;
        }
    }
}
