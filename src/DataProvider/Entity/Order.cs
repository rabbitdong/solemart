using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Solemart.SystemUtil;

namespace Solemart.DataProvider.Entity
{
    #region 订单项实体 OrderItem
    /// <summary>
    /// 订单项实体
    /// </summary>
#if TEST    
    [Table("TestOrderItems")] 
#endif
    public class OrderItem
    {
        public OrderItem()
        {
            OrderDetails = new List<OrderDetailItem>();
        }

        /// <summary>
        /// 订单号
        /// </summary>
        [Key]
        public int OrderID { get; set; }

        /// <summary>
        /// 订单的用户ID
        /// </summary>
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserItem User { get; set; }

        /// <summary>
        /// 订单的建立时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 订单的发货时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 订单的收货时间
        /// </summary>
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 订单的取消时间
        /// </summary>
        public DateTime CancelTime { get; set; }

        /// <summary>
        /// 订单的退货时间
        /// </summary>
        public DateTime RejectTime { get; set; }

        /// <summary>
        /// 订单的总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 订单的状态信息
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The order sending address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The phone number for the receiver.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The post code for the receiver.
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// The order receiver name.
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 该订单的网上支付工具产生的订单号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// The deliver way of the order
        /// </summary>
        public DeliverWay DeliverWay { get; set; }

        /// <summary>
        /// The payment type of the order
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Indicate the user pay the order or not
        /// </summary>
        public bool HasPay { get; set; }

        /// <summary>
        /// The appraise by the user
        /// </summary>
        public string Appraise { get; set; }

        /// <summary>
        /// The remark of the order
        /// </summary>
        public string Remark { get; set; }

        public virtual ICollection<OrderDetailItem> OrderDetails { get; set; }
    }
    #endregion

    #region 订单的产品项  OrderDetailItem
    /// <summary>
    /// The order detail item
    /// </summary>
#if TEST 
    [Table("TestOrderDetailItems")]
#endif
    public class OrderDetailItem
    {
        /// <summary>
        /// The orderid of the detail item
        /// </summary>
        [Key, Column(Order=0)]
        public int OrderID { get; set; }

        [ForeignKey("OrderID")]
        public virtual OrderItem Order { get; set; }

        /// <summary>
        /// The product id of the detail item
        /// </summary>
        [Key, Column(Order=1)]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductItem Product { get; set; }

        /// <summary>
        /// The amount of the product
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The unit price of the product
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The remark of the order item.
        /// </summary>
        public string Remark { get; set; }
    }
    #endregion

    #region 送货地址项实体 SendAddressItem
    /// <summary>送货地址项实体
    /// </summary>
#if TEST 
    [Table("TestSendAddressItems")] 
#endif
    public class SendAddressItem
    {
        /// <summary>
        /// 送货地址信息中的用户ID
        /// </summary>
        [Key]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserItem User { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收货人电话号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 收货人邮编信息
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>获取或设置送货方式
        /// </summary>
        public DeliverWay DeliverWay { get; set; }

        /// <summary>获取或设置付款方式
        /// </summary>
        public PaymentType PaymentType { get; set; }
    }
    #endregion

    #region 订单或购物车中的商品项对象 CartItem
    /// <summary>
    /// 订单或购物车中的商品项对象
    /// </summary>
#if TEST 
    [Table("TestCartItems")] 
#endif
    public class CartItem
    {
        /// <summary>
        /// the user id of the cart
        /// </summary>
        [Key, Column(Order=0)]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserItem User { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [Key, Column(Order=1)]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductItem Product { get; set; }

        /// <summary>
        /// 该商品的数量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 该商品的单价信息
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 商品项的备注信息
        /// </summary>
        public string Description { get; set; }
    }
    #endregion
}
