using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.EntityLib;
using Solemart.DataAccessLib;

namespace Solemart.BusinessLib {
    /// <summary>订单处理类
    /// </summary>
    public class OrderManager {
        private static OrderManager instance = new OrderManager();
        private ProductManager pm = ProductManager.Instance;
        private const int ORDER_CACHE_SIZE = 128;

        private OrderDA oda = OrderDA.Instance;

        private Queue<OrderItem> OrderCache = new Queue<OrderItem>(ORDER_CACHE_SIZE);

        /// <summary>默认的分页数目（每页显示的订单数目）
        /// </summary>
        private const int DEFAULT_PAGED_COUNT = 10;

        private int page_size = DEFAULT_PAGED_COUNT;

        /// <summary>获取订单管理对象的实例
        /// </summary>
        public static OrderManager Instance {
            get { return instance; }
        }

        /// <summary>获取或设置每次调用GetPagedOrders返回的订单的最大数目
        /// </summary>
        public int PageSize {
            get { return page_size; }
            set { page_size = value; }
        }

        private OrderManager() { }

        /// <summary>生成一个新订单
        /// </summary>
        /// <param name="order_item">该订单的产品列表</param>
        /// <returns>新建订单的订单ID，执行失败返回-1</returns>
        public int NewOrder(OrderItem order_item) {
            //更新没产品的单价信息
            foreach (ProductItem pi in order_item.Products) {
                pi.UnitPrice = pi.Product.SalePrice * pi.Product.Discount / 100;
            }

            int order_id = oda.SaveOrderInfo(order_item.OwnedUser.UserID, order_item);

            if (order_id != -1) {
                order_item.OrderID = order_id;
                pm.ReserveProducts(order_item.Products);
                UpdateNewOrderItem(order_item);
                return order_id;
            }

            return -1;
        }

        /// <summary>更新最近访问的订单对象
        /// </summary>
        /// <param name="new_order_item">要更新的订单对象</param>
        private void UpdateNewOrderItem(OrderItem new_order_item) {
            if (OrderCache.Count >= ORDER_CACHE_SIZE) {
                OrderItem last_oi = OrderCache.Dequeue();
                last_oi = null;
            }
            OrderCache.Enqueue(new_order_item);
        }

        /// <summary>从临时订单信息中获取订单对象
        /// </summary>
        /// <param name="toi">临时订单信息</param>
        /// <returns>返回获取到的订单对象</returns>
        private OrderItem GetOrderItemFromTmpOrderItem(TmpOrderItem toi) {
            OrderItem oi = new OrderItem();
            oi.OrderID = toi.OrderID;
            oi.OwnedUser = UserManager.Instance.GetUserByID(toi.UserID);
            oi.OrderTime = toi.OrderTime;
            oi.SendTime = toi.SendTime;
            oi.ReceiveTime = toi.ReceiveTime;
            oi.TotalPrice = toi.Price;
            oi.Status = toi.Status;
            oi.AddressInfo = new SendAddressInfo();
            oi.AddressInfo.Address = toi.Address;
            oi.AddressInfo.Post = toi.PostCode;
            oi.AddressInfo.Phone1 = toi.Phone1;
            oi.AddressInfo.Receiver = toi.ReceiverName;
            oi.AddressInfo.Channel = toi.Channel;
            oi.AddressInfo.Pay = toi.Pay;
            oi.IsPay = toi.IsPay;
            oi.TradeNo = toi.TradeNo;
            oi.Remark = toi.Remark;
            return oi;
        }

        /// <summary>获取某个订单的信息
        /// </summary>
        /// <param name="order_id">某个订单的订单ID</param>
        /// <returns>如果获取订单，返回该订单对象，否则返回null</returns>
        public OrderItem GetOrderInfo(int order_id) {
            OrderItem order_item = null;
            if (OrderCache.Count > 0 && OrderCache.Any(oi=>oi.OrderID == order_id))
                order_item = OrderCache.First(oi => oi.OrderID == order_id);

            if (order_item == null || order_item.Products == null || order_item.Products.Count() == 0) {
                TmpOrderItem toi = oda.GetOrderItem(order_id);
                if (toi != null) {
                    order_item = GetOrderItemFromTmpOrderItem(toi);
                    int[] order_ids = { toi.OrderID };

                    List<TmpProductItem> products = oda.GetOrderProductList(order_ids);
                    order_item.Products = from p in products
                                          select new ProductItem(pm.GetProductByID(p.ProductID), p.UnitPrice, p.Amount);
                }
            }

            return order_item;
        }

        /// <summary>获取用户第几页的订单列表
        /// </summary>
        /// <param name="user">要获取订单的用户</param>
        /// <param name="page_size">分页的大小，页索引从0开始</param>
        /// <returns>订单列表</returns>
        public List<OrderItem> GetUserPagedOrder(User user, int page_index) {
            List<TmpOrderItem> orders = oda.GetUserPagedOrder(user.UserID, page_index, page_size);
            List<OrderItem> order_list = new List<OrderItem>(orders.Count);
            foreach (TmpOrderItem toi in orders) {
                order_list.Add(GetOrderItemFromTmpOrderItem(toi));
            }

            return order_list;
        }

        /// <summary>获取系统中的总订单数
        /// </summary>
        /// <param name="order_status">订单的类别</param>
        /// <returns>获取到的订单总数</returns>
        public int GetTotalOrderNum(OrderStatus order_status) {
            return oda.GetTotalOrderNum(order_status);
        }

        /// <summary>获取某个用户的某种类型订单的总数目
        /// </summary>
        /// <param name="user">获取的用户</param>
        /// <param name="status">获取的订单的状态</param>
        /// <returns>获取到的订单的数量</returns>
        /// <remarks>如果status值是AllStatus，获取该用户的所有订单的数量</remarks>
        public int GetUserOrderCount(User user, OrderStatus status) {
            if (status == OrderStatus.AllStatus)
                return oda.GetUserOrderCount(user.UserID);
            else
                return oda.GetUserOrderCount(user.UserID, status);
        }

        /// <summary>获取订单用户的订单列表（分页获取）
        /// </summary>
        /// <param name="page_index">获取订单的页索引(从0开始)</param>
        /// <param name="page_size">分页的大小</param>
        /// <returns>获取到的订单列表(按时间倒序排列)</returns>
        public List<OrderItem> GetPagedOrders(OrderStatus status, int page_index) {
            List<TmpOrderItem> orders = oda.GetPagedOrders(status, page_size, page_index);
            List<OrderItem> get_orders = new List<OrderItem>();
            int[] order_ids = new int[orders.Count];
            for (int i=0; i<orders.Count; ++i) {
                get_orders.Add(GetOrderItemFromTmpOrderItem(orders[i]));
                order_ids[i] = orders[i].OrderID;
            }

            List<TmpProductItem> products = oda.GetOrderProductList(order_ids);
            foreach (OrderItem oi in get_orders) {
                oi.Products = from p in products where p.OrderID == oi.OrderID
                              select new ProductItem(ProductManager.Instance.GetProductByID(p.ProductID), p.UnitPrice, p.Amount);
            }
            return get_orders;    
        }

        /// <summary>对某个订单进行发货处理
        /// </summary>
        /// <param name="order_id">要发货的订单的订单号</param>
        /// <returns>是否发货成功</returns>
        public bool SendOrder(int order_id) {
            OrderItem oi = GetOrderInfo(order_id);
            //如果订单不对，不能进行发货处理
            if (oi == null || oi.Status != OrderStatus.Ordered)
                return false;

            if (oda.SendOrder(order_id)) {
                oi.Status = OrderStatus.Sending;
                pm.ShippingProducts(oi.Products);
                return true;
            }

            return false;
        }

        /// <summary>用户对订单进行支付
        /// </summary>
        /// <param name="order_id">要支付的订单的ID</param>
        /// <returns>是否成功支付</returns>
        public bool PayOrder(int order_id, string trade_no) {
            OrderItem oi = GetOrderInfo(order_id);
            if (oi == null || oi.IsPay)
                return false;

            if (oda.PayOrder(order_id, trade_no)) {
                oi.IsPay = true;
                return true;
            }

            return false;
        }

        /// <summary>用户对订单进行支付
        /// </summary>
        /// <param name="order_id">要支付的订单的ID</param>
        /// <returns>是否成功支付</returns>
        public bool PayOrder(int order_id) {
            OrderItem oi = GetOrderInfo(order_id);
            if (oi == null || oi.IsPay)
                return false;

            if (oda.PayOrder(order_id)) {
                oi.IsPay = true;
                return true;
            }

            return false;
        }

        /// <summary>用户对订单进行确认
        /// </summary>
        /// <param name="order_id">要确认的订单ID</param>
        /// <returns>用户的确认结果</returns>
        public bool ConfirmOrder(int order_id) {
            OrderItem oi = GetOrderInfo(order_id);
            if (oi == null || oi.Status != OrderStatus.Sending)
                return false;
            
            if (oda.ConfirmOrder(order_id)) {
                oda.PayOrder(order_id);
                oi.Status = OrderStatus.Received;
                oi.IsPay = true;
                return true;
            }

            return false;
        }

        /// <summary>获取新订单的数目(未发货的订单)
        /// </summary>
        public int GetNewOrderCount(){
            return oda.GetNewOrderCount();
        }

        /// <summary>用户取消订单
        /// </summary>
        /// <param name="order_id">要取消的订单ID</param>
        /// <returns>是否取消成功</returns>
        public bool CancelOrder(int order_id) {
            OrderItem oi = GetOrderInfo(order_id);
            if (oi == null || oi.Status != OrderStatus.Ordered)
                return false;

            if (oda.CancelOrder(order_id)) {
                pm.UnReserveProducts(oi.Products);
                oi.Status = OrderStatus.Cancel;
                return true;
            }

            return false;
        }
    }
}
