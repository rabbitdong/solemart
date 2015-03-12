using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Solemart.Web.Models
{
    /// <summary>
    /// 用于商品列表显示的ViewModel
    /// </summary>
    public class ProductForListViewModel
    {
        public int ProductID { get; set; }

        public string ProductImageUrl { get; set; }

        public string ProductImageName { get; set; }

        public bool IsSpecial { get; set; }

        public int Discount { get; set; }

        public decimal Price { get; set; }

        public string Unit { get; set; }

        public bool IsOutOfStock { get; set; }
    }
}