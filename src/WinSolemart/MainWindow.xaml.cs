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
            dgOrder.ItemsSource = orders;
        }
    }
}
