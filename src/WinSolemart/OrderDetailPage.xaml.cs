using Solemart.DataProvider.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// Interaction logic for OrderDetail.xaml
    /// </summary>
    public partial class OrderDetailPage : Page
    {
        public OrderDetailPage(OrderListViewModel model)
        {
            InitializeComponent();

            OrderDetailViewModel detailModel = new OrderDetailViewModel();
            detailModel.OrderID = model.OrderID;
            detailModel.Receiver = model.Receiver;
            detailModel.ReceiverPhone = model.ReceiverPhone;
            detailModel.UserName = model.UserName;
            detailModel.TotalPrice = model.TotalAmount;
            detailModel.Address = model.Address;
            detailModel.DetailItems = new List<OrderDetailItemViewModel>();

            foreach (OrderDetailItem orderDetail in model.OrderDetails)
            {
                detailModel.DetailItems.Add(new OrderDetailItemViewModel
                {
                    ProductName = orderDetail.Product.ProductName,
                    AmountString = string.Format("{0}({1})", orderDetail.Amount, orderDetail.Product.Unit),
                    UnitPrice = orderDetail.UnitPrice,
                    TotalPrice = orderDetail.UnitPrice * orderDetail.Amount
                });
            }

            tbOrderDetail.ItemsSource = detailModel.DetailItems;

            txtUserInfo.Text = string.Format("用户：{0}", detailModel.Receiver);
            txtSendAddress.Text = string.Format("送货地址：{0} 电话：{1}", detailModel.Address, detailModel.ReceiverPhone);
            txtTotalPrice.Text = string.Format("总金额：{0}", detailModel.TotalPrice);
        }

        /// <summary>
        /// Print the order detail.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(gdOrerDetailContent, "乐道订单");
            }
        }

        private void btnSendOrder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
