using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Solemart.DataProvider.Entity;
using Solemart.Web.Models;

namespace Solemart.Web.Areas.Manager.Models
{
    public class OrderManagerViewModel: IPagerModel
    {
        public List<OrderItem> OrderList { get; set; }

        public int PageIndex { get; set; }

        public int TotalPageCount { get; set; }
    }
}