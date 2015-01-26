using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xxx.EntityLib {
    /// <summary>订单对象
    /// </summary>
    public class OrderItem {
        /// <summary>获取或设置订单号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>获取或设置订单的用户
        /// </summary>
        public User OwnedUser { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime SendTime { get; set; }

        public DateTime ReceiveTime { get; set; }

        public DateTime CancelTime { get; set; }

        public decimal ProductPrice { get; set; }

        public decimal TotalPrice { get; set; }

        /// <summary>获取或设置订单的状态信息
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>该订单是否已经付款
        /// </summary>
        public bool IsPay { get; set; }

        /// <summary>该订单的网上支付工具产生的订单号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>获取或设置订单中的商品对象
        /// </summary>
        public IEnumerable<ProductItem> Products { get; set; }

        /// <summary>获取或设置订单的送货信息
        /// </summary>
        public SendAddressInfo AddressInfo { get; set; }

        /// <summary>获取或设置订单中的说明(用户的产品备注信息)
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>送货方式
    /// </summary>
    public enum ChannelType{
        /// <summary>送货上门
        /// </summary>
        [StringValue("送货上门")]
        ByManual,

        /// <summary>快递
        /// </summary>
        [StringValue("快递")]
        ByExpress,

        /// <summary>平邮
        /// </summary>
        [StringValue("平邮")]
        ByPost
    }

    /// <summary>付款方式
    /// </summary>
    public enum PayType {
        /// <summary>货到付款
        /// </summary>
        [StringValue("货到付款")]
        CashOnDelivery = 0,
        /// <summary>网上支付
        /// </summary>
        [StringValue("支付宝支付")]
        OnLine = 1,
        /// <summary>转账
        /// </summary>
        [StringValue("银行转账")]
        Transfer = 2
    }

    /// <summary>订单状态
    /// </summary>
    public enum OrderStatus {
        /// <summary>一个特殊的状态，他可以表示所有状态
        /// </summary>
        [StringValue("所有状态")]
        AllStatus = -1,

        /// <summary>下订单，还没有发货
        /// </summary>
        [StringValue("未发货")]
        Ordered,
        /// <summary>订单被取消
        /// </summary>
        [StringValue("已取消")]
        Cancel,
        /// <summary>已经发货，在送货过程中
        /// </summary>
        [StringValue("已发货")]
        Sending,
        /// <summary>用户已经收到商品
        /// </summary>
        [StringValue("已收货")]
        Received,
        /// <summary>订单被退货
        /// </summary>
        [StringValue("已退货")]
        Rejected
    }

    /// <summary>表示送货地址对象
    /// </summary>
    public class SendAddressInfo {
        /// <summary>获取或设置送货地址信息中的用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>获取或设置收货人姓名
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>收货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>收货人电话号码1
        /// </summary>
        public string Phone1 { get; set; }

        /// <summary>收货人邮编信息
        /// </summary>
        public string Post { get; set; }

        /// <summary>获取或设置送货方式
        /// </summary>
        public ChannelType Channel { get; set; }

        /// <summary>获取或设置付款方式
        /// </summary>
        public PayType Pay { get; set; }
    }

    /// <summary>订单对象，用于数据库访问到业务层的转换对象
    /// </summary>
    public class TmpOrderItem {
        /// <summary>获取或设置订单的ID
        /// </summary>
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime SendTime { get; set; }

        public DateTime ReceiveTime { get; set; }

        /// <summary>获取或设置订单的状态信息
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>订单的价格信息
        /// </summary>
        public decimal Price { get; set; }

        public string Address { get; set; }

        public string Phone1 { get; set; }

        public string PostCode { get; set; }

        public string ReceiverName { get; set; }

        /// <summary>获取或设置订单的送货渠道
        /// </summary>
        public ChannelType Channel { get; set; }

        /// <summary>获取或设置订单的付款方式
        /// </summary>
        public PayType Pay { get; set; }

        /// <summary>获取或设置该订单是否已经付款
        /// </summary>
        public bool IsPay { get; set; }

        /// <summary>获取或设置该订单的网上订单号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>获取或设置订单的评价信息
        /// </summary>
        public string Appraise { get; set; }

        /// <summary>获取或设置订单的产品说明(用户的产品备注信息)
        /// </summary>
        public string Remark { get; set; }

        /// <summary>该订单中的商品项列表
        /// </summary>
        public List<TmpProductItem> Products { get; set; }
    }

    /// <summary>订单中商品项（商品ID和该商品的数量），用于数据访问对象和业务对象间的数据传递
    /// </summary>
    public class TmpProductItem {
        public int OrderID { get; set; }

        /// <summary>获取或设置产品的ID
        /// </summary>
        public int ProductID { get; set; }

        public int Amount { get; set; }

        /// <summary>获取或设置该商品的单价信息
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>订单或购物车中的商品项对象
    /// </summary>
    public class ProductItem {
        public ProductItem() { }

        /// <summary>生成一个商品项
        /// </summary>
        /// <param name="product">商品对象</param>
        /// <param name="amount">该商品的数量</param>
        public ProductItem(Product product, decimal unit_price, int amount) {
            Product = product;
            Amount = amount;
            UnitPrice = unit_price;
        }

        /// <summary>商品
        /// </summary>
        public Product Product { get; set; }

        /// <summary>该商品的数量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>该商品的单价信息
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
