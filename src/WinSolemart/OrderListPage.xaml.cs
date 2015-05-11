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
using System.Windows.Threading;
using WinSolemart.Models;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for OrderList.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {
        private DispatcherTimer timer;
        IEnumerable<OrderListViewModel> model = null;

        public OrderListPage()
        {
            InitializeComponent();

            cmbOrderStatus.ItemsSource = EnumConstantList.OrderStatusList;
            cmbOrderStatus.SelectedIndex = 0;

            int totalCount = 0;
            List<OrderItem> orders = OrderManager.GetPagedOrders(OrderStatus.Ordered, 0, 10, out totalCount);
            model = orders.Select(o => new OrderListViewModel
            {
                OrderID = o.OrderID,
                UserName = o.User.UserName,
                OrderStatus = o.Status,
                Address = o.Address,
                Receiver = o.Receiver,
                OrderTime = o.OrderTime,
                ReceiverPhone = o.Phone,
                TotalAmount = o.TotalPrice,
                OrderDetails = o.OrderDetails
            });

            dgOrder.ItemsSource = model;

            timer = new DispatcherTimer();
            timer.Tick += dispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0,0,5);
            timer.Start();
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            OrderListViewModel model = link.DataContext as OrderListViewModel;
            
            NavigationService.Navigate(new OrderDetailPage(model));
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            BindedEnumItem item = cmbOrderStatus.SelectedItem as BindedEnumItem;
            if ((OrderStatus)item.enumValue != OrderStatus.Ordered)
                return;

            int totalCount = 0;
            List<OrderItem> orders = OrderManager.GetPagedOrders(OrderStatus.Ordered, 0, 10, out totalCount);
            model = orders.Select(o => new OrderListViewModel
            {
                OrderID = o.OrderID,
                UserName = o.User.UserName,
                OrderStatus = o.Status,
                Address = o.Address,
                Receiver = o.Receiver,
                OrderTime = o.OrderTime,
                ReceiverPhone = o.Phone,
                TotalAmount = o.TotalPrice,
                OrderDetails = o.OrderDetails
            });

            dgOrder.ItemsSource = model;
            dgOrder.Items.Refresh();
        }

        private void btnGetOrder_Click(object sender, RoutedEventArgs e)
        {
            BindedEnumItem item = cmbOrderStatus.SelectedItem as BindedEnumItem;

            int totalCount = 0;
            List<OrderItem> orders = OrderManager.GetPagedOrders((OrderStatus)item.enumValue, 0, 10, out totalCount);
            model = orders.Select(o => new OrderListViewModel
            {
                OrderID = o.OrderID,
                UserName = o.User.UserName,
                OrderStatus = o.Status,
                Address = o.Address,
                Receiver = o.Receiver,
                OrderTime = o.OrderTime,
                ReceiverPhone = o.Phone,
                TotalAmount = o.TotalPrice,
                OrderDetails = o.OrderDetails
            });

            dgOrder.ItemsSource = model;
            dgOrder.Items.Refresh();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
