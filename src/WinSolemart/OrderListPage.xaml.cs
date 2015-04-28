using Solemart.BusinessLib;
using Solemart.DataProvider.Entity;
using Solemart.SystemUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinSolemart.Models;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for OrderList.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {
        public OrderListPage()
        {
            InitializeComponent();

            int totalCount = 0;
            List<OrderItem> orders = OrderManager.GetPagedOrders(OrderStatus.Ordered, 0, 10, out totalCount);
            IEnumerable<OrderListViewModel> model = orders.Select(o => new OrderListViewModel { OrderID = o.OrderID.ToString(), UserName = o.User.UserName, Address = o.Address, Receiver = o.Receiver, ReceiverPhone = o.Phone, TotalAmount = o.TotalPrice, OrderDetails=o.OrderDetails });
            dgOrder.ItemsSource = model;
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            OrderListViewModel model = link.DataContext as OrderListViewModel;
            
            NavigationService.Navigate(new OrderDetailPage(model));
        }
    }
}
