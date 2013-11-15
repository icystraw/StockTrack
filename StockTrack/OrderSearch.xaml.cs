using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockTrack
{
    /// <summary>
    /// Interaction logic for OrderSearch.xaml
    /// </summary>
    public partial class OrderSearch : Window
    {
        public OrderSearch()
        {
            InitializeComponent();
            this.Width = Properties.Settings.Default.osWidth;
            this.Height = Properties.Settings.Default.osHeight;
            this.Left = Properties.Settings.Default.osXpos;
            this.Top = Properties.Settings.Default.osYpos;
        }

        private void dgOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                Order o = dgOrders.SelectedItem as Order;
                OrderDetails od = new OrderDetails();
                od.OrderId = o.OrderId;
                od.Owner = this;
                od.ShowDialog();
                performSearch();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            performSearch();
        }

        private void performSearch()
        {
            bool? isWorkOrder = null;
            if (cbIsWorkOrder.SelectedIndex == 1) isWorkOrder = true;
            else if (cbIsWorkOrder.SelectedIndex == 2) isWorkOrder = false;
            dgOrders.ItemsSource = DataAccess.SearchOrder(txtOrderNo.Text.Trim(), txtKeyword.Text.Trim(), cbShipping.Text.Trim(), dtOrderDate1.SelectedDate, dtOrderDate2.SelectedDate, dtShippingDate1.SelectedDate, dtShippingDate2.SelectedDate, isWorkOrder);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.osWidth = (int)this.Width;
            Properties.Settings.Default.osHeight = (int)this.Height;
            Properties.Settings.Default.osXpos = (int)this.Left;
            Properties.Settings.Default.osYpos = (int)this.Top;
            Properties.Settings.Default.Save();
        }

        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {
            Mini m = new Mini();
            m.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            performSearch();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Name n = new Name();
            n.Owner = this;
            n.CustomTitle = "Enter New Order Number:";
            if (n.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(n.Input))
                {
                    return;
                }
                if (DataAccess.GetOrderByNo(n.Input).Count > 0)
                {
                    if (MessageBox.Show("The order already exists. Do you want to add another order under the same order number?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;
                }
                int orderId = DataAccess.AddNewWorkOrder(n.Input);
                if (orderId > 0)
                {
                    OrderDetails od = new OrderDetails();
                    od.OrderId = orderId;
                    od.Owner = this;
                    od.ShowDialog();
                    performSearch();
                }
            }
        }

        private void hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hyperlink link = (Hyperlink)e.OriginalSource;
                Process.Start(link.NavigateUri.AbsoluteUri);
            }
            catch { }
        }

        private void dgOrders_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            MessageBox.Show((e.Row.Item as Order).Comments);
        }
    }
}
