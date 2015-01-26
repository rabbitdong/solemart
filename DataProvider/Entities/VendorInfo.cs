using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xxx.EntityLib {
    /// <summary>表示供应商信息的缓存对象，目前只包含ID和供应商名称
    /// </summary>
    public class VendorInfoCache {
        public int VendorID { get; set; }
        public string VendorName { get; set; }
    }

    /// <summary>表示供应商信息的对象
    /// </summary>
    public class VendorInfo {
        /// <summary>获取或设置供应商的ID
        /// </summary>
        public int VendorID { get; set; }

        /// <summary>获取或设置供应商名称（公司名称）
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>获取或设置供应商的地址信息
        /// </summary>
        public string Address { get; set; }

        /// <summary>获取或设置供应商的联系人姓名
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>获取或设置供应商的网站的地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>获取或设置供应商的Email信息
        /// </summary>
        public string Email { get; set; }

        /// <summary>获取或设置供应商的其他联系信息
        /// </summary>
        public string OtherContact { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        /// <summary>获取或设置供应商的录入时间
        /// </summary>
        public DateTime RecordTime { get; set; }

        /// <summary>获取或设置该供应商的评价信息
        /// </summary>
        public string Evaluation { get; set; }
    }
}
