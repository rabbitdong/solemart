using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Solemart.DataProvider.Entity
{
    #region 供应商信息的对象  VendorItem
    /// <summary>
    /// 供应商信息的对象
    /// </summary>
    public class VendorItem {
        /// <summary>
        /// 获取或设置供应商的ID
        /// </summary>
        [Key]
        public int VendorID { get; set; }

        /// <summary>
        /// 获取或设置供应商名称（公司名称）
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 获取或设置供应商的地址信息
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 获取或设置供应商的联系人姓名
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 获取或设置供应商的网站的地址
        /// </summary>
        public string VendorUrl { get; set; }

        /// <summary>
        /// 获取或设置供应商的Email信息
        /// </summary>
        public string VendorEmail { get; set; }

        /// <summary>
        /// 供应商的联系电话号码
        /// </summary>
        public string VendorPhone { get; set; }

        /// <summary>
        /// 获取或设置供应商的录入时间
        /// </summary>
        public DateTime RecordTime { get; set; }

        /// <summary>
        /// 获取或设置该供应商的评价信息
        /// </summary>
        public string Evaluation { get; set; }

        /// <summary>
        /// The extention content for the vendor(JSON)
        /// </summary>
        public string ExtContent { get; set; }
    }
    #endregion
}
