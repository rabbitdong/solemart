using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Solemart.SystemUtil;

namespace Solemart.DataProvider.Entity
{
    #region 产品的评价项实体 ProductCommentItem
    /// <summary>
    /// 产品的评价项实体
    /// </summary>
    [Table("productcommentitems")]
    public class ProductCommentItem
    {
        [Key]
        public int CommentID { get; set; }

        /// <summary>
        /// 该评论的用户
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// The user info of the comment.
        /// </summary>
        [ForeignKey("UserID")]
        public virtual UserItem UserItem { get; set; }

        /// <summary>
        /// 该评论所评论的商品
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 该评论的星级
        /// </summary>
        public EvaluteGrade Grade { get; set; }

        /// <summary>
        /// 该评论的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 该评论的时间
        /// </summary>
        public DateTime CommentTime { get; set; }
    }
    #endregion

    #region 商品类别项实体 CategoryItem
    /// <summary>
    /// 商品类别项实体
    /// </summary>
    [Table("categoryitems")]
    public class CategoryItem
    {
        [Key]
        public int CategoryID { get; set; }

        /// <summary>
        /// The name of category
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// The description of the category
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The parent category id of the category, if no parent, the value is null.
        /// </summary>
        [ForeignKey("SubCategories")]
        public int? ParentCategoryID { get; set; }

        /// <summary>
        /// The child category list of the category.
        /// </summary>
        public IList<CategoryItem> SubCategories { get; set; }
    }
    #endregion

    #region 商品的品牌信息对象 BrandItem
    /// <summary>
    /// 商品的品牌项实体
    /// </summary>
    [Table("branditems")]
    public class BrandItem
    {
        /// <summary>
        /// 品牌的ID
        /// </summary>
        [Key]
        public int BrandID { get; set; }

        /// <summary>
        /// 品牌的名称
        /// </summary>
        public string ZhName { get; set; }

        /// <summary>
        /// 品牌的英文名称
        /// </summary>
        public string EnName { get; set; }

        /// <summary>
        /// 品牌的图片名称
        /// </summary>
        public string BrandLogo { get; set; }

        /// <summary>
        /// 品牌的描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 品牌的Url
        /// </summary>
        public string BrandUrl { get; set; }

        /// <summary>
        /// 受欢迎程度
        /// </summary>
        public int? Popularity { get; set; }
    }
    #endregion

    #region 商品的图片项实体 ProductImageItem
    /// <summary>
    /// 商品的图片项实体
    /// </summary>
    [Table("productimageitems")] 
    public class ProductImageItem
    {
        public static ProductImageItem NoImgProductImage = new ProductImageItem() { ImageID = -1, MimeType = "image/png", ImageUrl = "no-img.png" };

        [Key]
        public int ImageID { get; set; }

        /// <summary>
        /// The product id of the image item
        /// </summary>
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductItem Product { get; set; }

        public string MimeType { get; set; }

        /// <summary>
        /// 商品的图片的URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The description of the image item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否作为首页的图片显示
        /// </summary>
        public bool ForLogo { get; set; }

        /// <summary>
        /// 图片的添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
    #endregion

    #region 商品项对象 ProductItem
    /// <summary>
    /// 商品项对象
    /// </summary>
    [Table("productitems")] 
    public class ProductItem
    {
        /// <summary>
        /// 货品的ID
        /// </summary>
        [Key]
        public int ProductID { get; set; }

        /// <summary>
        /// The category id of the product.
        /// </summary>
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual CategoryItem Category { get; set; }
        /// <summary>
        /// 货品的名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 货品的描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 货品的规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 商品的单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 商品的库存数量
        /// </summary>
        public decimal StockCount { get; set; }

        /// <summary>
        /// 商品的预留数量（表示被订购，但还没发货的数量）
        /// </summary>
        public decimal ReserveCount { get; set; }

        /// <summary>
        /// 商品的品牌ID信息
        /// </summary>
        public int BrandID { get; set; }

        [ForeignKey("BrandID")]
        public virtual BrandItem Brand { get; set; }

        /// <summary>
        /// The first time of the product in stock
        /// </summary>
        public DateTime FirstInStockTime { get; set; }

        /// <summary>
        /// 本商品的供应商ID
        /// </summary>
        public int VendorID { get; set; }

        [ForeignKey("VendorID")]
        public virtual VendorItem Vendor { get; set; }

        /// <summary>
        /// The producting area of the product.
        /// </summary>
        public string ProducingArea { get; set; }

        /// <summary>
        /// The size of the product
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The color of the product
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The weight of the product
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// The extent content of the product(JSON)
        /// </summary>
        public string ExtContent { get; set; }

        public SaledProductItem SaledProduct { get; set; }
    }
    #endregion

    #region 销售产品项 SaledProductItem
    /// <summary>
    /// 销售产品项
    /// </summary>
    [Table("saledproductitems")]
    public class SaledProductItem
    {
        /// <summary>
        /// The product id of the saled product
        /// </summary>
        [Key, ForeignKey("Product")]
        public int ProductID { get; set; }

        public virtual ProductItem Product { get; set; }

        /// <summary>
        /// The sale price of the saled product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The discount of the saled product
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Indicate the saled product is special or not.
        /// </summary>
        public bool SpecialFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal SaledPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SetTop { get; set; }
    }
    #endregion

    #region 商品的价格历史项 PriceHistoryItem
    /// <summary>
    /// 商品的价格历史项 
    /// </summary>
    [Table("pricehistoryitems")]
    public class PriceHistoryItem
    {
        [Key]
        public int HistoryID { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductItem Product { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal Price { get; set; }
    }
    #endregion

    #region 商品的入库项 InStockItem
    /// <summary>
    /// The product instock item
    /// </summary>
    [Table("instockitems")]
    public class InStockItem
    {
        [Key]
        public int InStockID { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductItem Product { get; set; }

        public DateTime InStockTime { get; set; }

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public string Remark { get; set; }
    }
    #endregion
}
