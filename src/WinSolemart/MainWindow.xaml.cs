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
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.DataProvider.Entity;
using WinSolemart.Models;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            int totalCount = 0;
            List<OrderItem> orders = OrderManager.GetPagedOrders(OrderStatus.Ordered, 0, 10, out totalCount);
            IEnumerable<OrderListViewModel> model = orders.Select(o => new OrderListViewModel { OrderID = o.OrderID.ToString(), UserName = o.User.UserName, Address = o.Address, Receiver = o.Receiver, ReceiverPhone = o.Phone, TotalAmount = o.TotalPrice });
            dgOrder.ItemsSource = model;
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            OrderListViewModel model = link.DataContext as OrderListViewModel;
            MessageBox.Show(model.OrderID.ToString());
        }
    }
}
