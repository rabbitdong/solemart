using Solemart.DataProvider.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Solemart.Web.Models
{
    public class ProductDetailViewModel
    {
        /// <summary>
        /// The product id.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// The name of product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The description of the product
        /// </summary>
        public string ProductDescription { get; set; }

        #region saled info
        /// <summary>
        /// The sale price of the saled product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The unit of the product.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The discount of the saled product
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Indicate the saled product is special or not.
        /// </summary>
        public bool SpecialFlag { get; set; }

        /// <summary>
        /// The remain amount of the product.
        /// </summary>
        public string RemainAmount { get; set; }
        #endregion

        #region brand info
        /// <summary>
        /// 品牌的名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 品牌的图片名称
        /// </summary>
        public string BrandLogo { get; set; }

        /// <summary>
        /// 品牌的描述信息
        /// </summary>
        public string BrandDescription { get; set; }

        /// <summary>
        /// 品牌的Url
        /// </summary>
        public string BrandUrl { get; set; }
        #endregion

        /// <summary>
        /// The comment count for the product
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// The images list of the product(no include logo picture)
        /// </summary>
        public List<ProductImageItem> Images { get; set; }
    }
}