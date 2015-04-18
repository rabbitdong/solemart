using Solemart.DataProvider.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Solemart.Web.Models
{
    public class OrderListViewModel : IPagerModel
    {
        public int PageIndex { get; set; }

        public int TotalPageCount { get; set; }

        public IList<OrderItem> OrderList { get; set; }
    }
}