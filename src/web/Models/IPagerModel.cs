using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Solemart.Web.Models
{
    /// <summary>
    /// 分页使用的Model
    /// </summary>
    public interface IPagerModel
    {
        int PageIndex { get; set; }

        int TotalPageCount { get; set; }
    }
}