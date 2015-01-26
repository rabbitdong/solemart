using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xxx.Web.Models {
    public class PagerModel {
        /// <summary>
        /// 获取或设置页面的数目
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 获取或设置当前页面的索引
        /// </summary>
        public int CurrentPageIndex { get; set; }
    }
}