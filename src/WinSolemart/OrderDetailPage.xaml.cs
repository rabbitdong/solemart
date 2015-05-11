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
using Solemart.BusinessLib;
using Solemart.DataProvider.Entity;
using Solemart.SystemUtil;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for OrderDetail.xaml
    /// </summary>
    public partial class OrderDetailPage : Page
    {
        private OrderDetailViewModel detailModel;
        private int numberRowOfOnePage;

        public OrderDetailPage(OrderListViewModel model)
        {
            InitializeComponent();

            if (model.OrderStatus != OrderStatus.Ordered)
            {
                btnSendOrder.Visibility = System.Windows.Visibility.Hidden;
                btnPrint.Content = "补打订单";
            }

            detailModel = new OrderDetailViewModel();
            detailModel.OrderID = model.OrderID;
            detailModel.Receiver = model.Receiver;
            detailModel.ReceiverPhone = model.ReceiverPhone;
            detailModel.UserName = model.UserName;
            detailModel.TotalPrice = model.TotalAmount;
            detailModel.Address = model.Address;
            detailModel.OrderTime = model.OrderTime;
            detailModel.DetailItems = new List<OrderDetailItemViewModel>();

            foreach (OrderDetailItem orderDetail in model.OrderDetails)
            {
                detailModel.DetailItems.Add(new OrderDetailItemViewModel
                {
                    ProductName = orderDetail.Product.ProductName,
                    ProductArea = orderDetail.Product.ProducingArea,
                    AmountString = string.Format("{0}({1})", orderDetail.Amount, orderDetail.Product.Unit),
                    UnitPrice = orderDetail.UnitPrice.ToString(),
                    TotalPrice = (orderDetail.UnitPrice * orderDetail.Amount).ToString()
                });
            }

            numberRowOfOnePage = 9;
            int addCount = numberRowOfOnePage - detailModel.DetailItems.Count;
            if (addCount < numberRowOfOnePage)
            {
                for (int i = 0; i < addCount; ++i)
                {
                    detailModel.DetailItems.Add(new OrderDetailItemViewModel { ProductName = "", ProductArea = "", AmountString = "", TotalPrice = "", UnitPrice = "" });
                }
            }


            tbOrderDetail.ItemsSource = detailModel.DetailItems;

            txtOrderID.Text = string.Format("订单号：{0:00000000}", detailModel.OrderID);
            txtUserInfo.Text = string.Format("客   户：{0}", detailModel.Receiver);
            txtReceiver.Text = string.Format("收  货 人：{0}", detailModel.Receiver);
            txtOrderDate.Text = string.Format("下单时间：{0:yyyy年M月d日 H小时}", detailModel.OrderTime);
            txtSendAddress.Text = string.Format("送货地址：{0} 电话：{1}", detailModel.Address, detailModel.ReceiverPhone);
            txtTotalPrice.Text = string.Format("总金额：{0}元", detailModel.TotalPrice);
        }

        /// <summary>
        /// Print the order detail.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintTicket pt = printDialog.PrintTicket;
            pt.PageMediaSize = new PageMediaSize(24.1, 14.0);
            pt.PageOrientation = PageOrientation.Portrait;
            printDialog.PrintVisual(gdOrerDetailContent, "乐道订单");
        }

        /// <summary>
        /// The event of send the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrderManager.SendOrder(detailModel.OrderID))
            {
                txtMsg.Text = "本订单已经发货！";
            }
        }
    }
}
