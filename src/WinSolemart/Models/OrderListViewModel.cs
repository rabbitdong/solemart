using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider.Entity;
using Solemart.SystemUtil;

namespace WinSolemart.Models
{
    public class OrderListViewModel
    {
        public int OrderID { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public DateTime OrderTime { get; set; }
        public string ActionViewString { 
            get { return "查看"; } 
        }

        public ICollection<OrderDetailItem> OrderDetails { get; set; }
    }

    public class OrderDetailItemViewModel
    {
        public string ProductName { get; set; }
        public string ProductArea { get; set; }
        public string AmountString { get; set; }
        public string UnitPrice { get; set; }
        public string TotalPrice { get; set; }
    }

    public class OrderDetailViewModel
    {
        public int OrderID { get; set; }
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public DateTime OrderTime { get; set; }

        public ICollection<OrderDetailItemViewModel> DetailItems { get; set; }
    }
}
