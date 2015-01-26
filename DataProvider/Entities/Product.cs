using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xxx.EntityLib {
    /// <summary>评价等级的枚举
    /// </summary>
    public enum EvaluteGrade {
        None = 0,       
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3,
        FourStar = 4,
        FiveStar = 5
    }

    /// <summary>表示产品的评价类
    /// </summary>
    public class ProductComment {
        /// <summary>该评论的用户
        /// </summary>
        public User User { get; set; }

        /// <summary>获取或设置该评论所评论的商品
        /// </summary>
        public Product Product { get; set; }

        /// <summary>该评论的星级
        /// </summary>
        public EvaluteGrade Grade { get; set; }

        /// <summary>获取或设置该评论的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>获取或设置该评论的时间
        /// </summary>
        public DateTime CommentTime { get; set; }
    }

    /// <summary>临时评论对象，用于数据访问到业务处理的传递对象
    /// </summary>
    public class TmpProductComment {
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public EvaluteGrade Grade { get; set; }
        public DateTime CommentTime { get; set; }
        public string Content { get; set; }
    }

    /// <summary>表示类别对象
    /// </summary>
    public class Category {
        public int CateID { get; set; }

        /// <summary>类别名称
        /// </summary>
        public string CateName { get; set; }

        /// <summary>类别的说明
        /// </summary>
        public string Desc { get; set; }

        /// <summary>分类级别
        /// </summary>
        public int CateLevel { get; set; }

        /// <summary>获取或设置该类别的产品的数量
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>这个类别下的所有子类别
        /// </summary>
        public List<Category> SubCategories { get; set; }

        /// <summary>该类别的父类别
        /// </summary>
        public Category SupCategory { get; set; }
    }

    /// <summary>品牌信息对象
    /// </summary>
    public class BrandInfo {
        /// <summary>获取或设置品牌的ID
        /// </summary>
        public int BrandID { get; set; }

        /// <summary>获取或设置品牌的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>品牌的英文名称
        /// </summary>
        public string EnName { get; set; }

        /// <summary>品牌的图片名称
        /// </summary>
        public string Image { get; set; }

        /// <summary>品牌的描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>品牌的Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>受欢迎程度
        /// </summary>
        public int Populer { get; set; }
    }

    /// <summary>表示商品的图片
    /// </summary>
    public class ProductImage {
        public static ProductImage NoImgProductImage = new ProductImage() { ImgID = -1, Imgtype = "image/png", Url = "no-img.png" };

        public int ImgID { get; set; }

        public string Imgtype { get; set; }

        /// <summary>商品的图片的URL
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>表示某个产品
    /// </summary>
    public class Product {
        /// <summary>获取或设置货品的ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>获取或设置货品的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>获取或设置货品的描述信息
        /// </summary>
        public string Desc { get; set; }

        /// <summary>获取或设置货品的规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>获取或设置商品的折扣信息
        /// </summary>
        public int Discount { get; set; }

        /// <summary>获取或设置商品的单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>获取或设置商品是否是特价商品
        /// </summary>
        public bool IsSpec { get; set; }

        /// <summary>获取或设置商品的销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>获取或设置商品的库存数量
        /// </summary>
        public int StockCount { get; set; }

        /// <summary>获取或设置商品的预留数量（表示被订购，但还没发货的数量）
        /// </summary>
        public int ReserceCount { get; set; }

        /// <summary>该商品是否在销售中
        /// </summary>
        public bool IsSaling { get; set; }

        /// <summary>获取或设置商品的品牌ID信息
        /// </summary>
        public int BrandID { get; set; }

        /// <summary>获取或设置该产品所属的类别
        /// </summary>
        public Category OwnedCategory { get; set; }

        public ProductImage[] Images { get; set; }

        /// <summary>获取或设置本商品的供应商ID
        /// </summary>
        public int VendorID { get; set; }

        /// <summary>获取或设置本产品下的所有评论
        /// </summary>
        public List<ProductComment> Comments { get; set; }
    }

    /// <summary>销售商品的临时对象。只用于数据访问
    /// </summary>
    public class TempSaleProduct {
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public bool IsSpecFlag { get; set; }

        /// <summary>销售商品的权重，它表示受欢迎程度
        /// </summary>
        public int Weight { get; set; }
    }
}
