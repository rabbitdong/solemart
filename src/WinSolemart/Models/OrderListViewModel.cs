using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSolemart.Models
{
    public class OrderListViewModel
    {
        public string OrderID { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public string ActionViewString { 
            get { return "查看"; } 
        }
    }
}
