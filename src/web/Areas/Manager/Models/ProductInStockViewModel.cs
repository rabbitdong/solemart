using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Solemart.Web.Areas.Manager.Models
{
    /// <summary>
    /// 商品入库的视图模型
    /// </summary>
    public class ProductInStockViewModel
    {
        /// <summary>
        /// 货品的ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// The category id of the product.
        /// </summary>
        public int CategoryID { get; set; }

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
        /// 商品的入库数量
        /// </summary>
        public int StockAmount { get; set; }

        /// <summary>
        /// 商品的入库价格
        /// </summary>
        public decimal StockPrice { get; set; }

        /// <summary>
        /// 商品的品牌ID信息
        /// </summary>
        public int BrandID { get; set; }

        /// <summary>
        /// 本商品的供应商ID
        /// </summary>
        public int VendorID { get; set; }

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
        /// The remark of this stock
        /// </summary>
        public string Remark { get; set; }
    }
}