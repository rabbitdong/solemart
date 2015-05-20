using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.SystemUtil
{
    #region 送货方式  DeliverGoodsMethod
    /// <summary>
    /// 送货方式
    /// </summary>
    public enum DeliverWay
    {
        /// <summary>
        /// 送货上门
        /// </summary>
        [EnumDisplay("送货上门")]
        ByManual,

        /// <summary>
        /// 快递
        /// </summary>
        [EnumDisplay("快递")]
        ByExpress,

        /// <summary>
        /// 平邮
        /// </summary>
        [EnumDisplay("平邮")]
        ByPost
    }
    #endregion

    #region 订单的付款方式  PaymentType
    /// <summary>
    /// 订单的付款方式
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// 货到付款
        /// </summary>
        [EnumDisplay("货到付款")]
        CashOnDelivery = 0,

        /// <summary>
        /// 支付宝支付
        /// </summary>
        [EnumDisplay("支付宝支付")]
        OnLine = 1,

        /// <summary>
        /// 银行转账
        /// </summary>
        [EnumDisplay("银行转账")]
        Transfer = 2
    }
    #endregion

    #region 订单状态  OrderStatus
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 一个特殊的状态，可以表示所有状态
        /// </summary>
        [EnumDisplay("所有状态")]
        AllStatus = -1,

        /// <summary>
        /// 下订单，还没有发货
        /// </summary>
        [EnumDisplay("未发货")]
        Ordered,

        /// <summary>
        /// 订单被取消
        /// </summary>
        [EnumDisplay("已取消")]
        Cancel,

        /// <summary>
        /// 已经发货，在送货过程中
        /// </summary>
        [EnumDisplay("已发货")]
        Sending,

        /// <summary>
        /// 用户已经收到商品
        /// </summary>
        [EnumDisplay("已收货")]
        Received,

        /// <summary>
        /// 订单被退货
        /// </summary>
        [EnumDisplay("已退货")]
        Rejected
    }
    #endregion

    #region 评价等级  EvaluteGrade
    /// <summary>
    /// 评价等级
    /// </summary>
    public enum EvaluteGrade
    {
        None = 0,
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3,
        FourStar = 4,
        FiveStar = 5
    }
    #endregion

    #region 表示用户性别信息  Sex
    /// <summary>
    /// 表示用户性别信息
    /// </summary>
    public enum Sex
    {
        /// <summary>
        /// 表示该用户的性别未知，（没有填写)
        /// </summary>
        [EnumDisplay("未知")]
        Unknown,
        /// <summary>
        /// 男性
        /// </summary>
        [EnumDisplay("男")]
        Male,
        /// <summary>
        /// 女性
        /// </summary>
        [EnumDisplay("女")]
        Female
    }
    #endregion

    #region 用户的积分类型
    /// <summary>
    /// 表示用户积分类型
    /// </summary>
    public enum PointType
    {
        /// <summary>
        /// 购买物品积分
        /// </summary>
        BuyGoods = 1,
        /// <summary>
        /// 花费积分用于购买物品
        /// </summary>
        ConsumeGoods  = 2,
        /// <summary>
        /// 退货扣除积分
        /// </summary>
        ReturnGoods = 3,
    }
    #endregion

    #region 用户的登录类型  LoginType
    /// <summary>用户的登录类型（可以从其它网站的认证登录）
    /// </summary>
    public enum LoginType
    {
        /// <summary>用户从本地网站登录
        /// </summary>
        [EnumDisplay("本地用户")]
        Local = 0,

        /// <summary>用户从QQ登录
        /// </summary>
        [EnumDisplay("QQ用户")]
        QQ = 1,

        /// <summary>
        /// 从微信来的用户
        /// </summary>
        [EnumDisplay("Weixin用户")]
        Weixin = 2
    }
    #endregion

    /// <summary>
    /// The enum item used for the binding.
    /// </summary>
    public class BindedEnumItem
    {
        public Enum enumValue;

        public override string ToString()
        {
            return enumValue.ToDisplayStr();
        }
    }

    public static class EnumConstantList
    {
        /// <summary>
        /// Get all the order status enumeration.
        /// </summary>
        public static BindedEnumItem[] OrderStatusList = {new BindedEnumItem{enumValue=OrderStatus.Ordered},
                                                             new BindedEnumItem{enumValue=OrderStatus.Sending},
                                                             new BindedEnumItem{ enumValue=OrderStatus.Cancel},                                                         
                                                         new BindedEnumItem{enumValue=OrderStatus.Received}
                                                         };
    }
}
