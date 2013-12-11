using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            performSearch(0);
        }

        private void performSearch(int orderId)
        {
            bool? isWorkOrder = null;
            if (cbIsWorkOrder.SelectedIndex == 1) isWorkOrder = true;
            else if (cbIsWorkOrder.SelectedIndex == 2) isWorkOrder = false;
            SortDescription sdPrimary = new SortDescription("ShippingDate", ListSortDirection.Ascending);
            if (dgOrders.Items.SortDescriptions.Count > 0)
            {
                sdPrimary = dgOrders.Items.SortDescriptions[0];
            }
            dgOrders.CancelEdit();
            int selectedOrderId = 0;
            if (dgOrders.SelectedItem != null)
            {
                Order o = dgOrders.SelectedItem as Order;
                selectedOrderId = o.OrderId;
            }
            dgOrders.ItemsSource = DataAccess.SearchOrder(txtOrderNo.Text.Trim(), txtKeyword.Text.Trim(), cbShipping.Text.Trim(), dtOrderDate1.SelectedDate, dtOrderDate2.SelectedDate, dtShippingDate1.SelectedDate, dtShippingDate2.SelectedDate, isWorkOrder);
            dgOrders.Items.SortDescriptions.Add(sdPrimary);
            if (selectedOrderId == 0 && orderId == 0) return;
            foreach (Order o in dgOrders.Items)
            {
                if (o.OrderId == orderId || o.OrderId == selectedOrderId)
                {
                    dgOrders.SelectedItem = o;
                    break;
                }
            }
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
            performSearch(0);
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
                    performSearch(orderId);
                }
            }
        }

        private void dgOrders_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Order o = e.Row.Item as Order;
            DataAccess.UpdateOrder(o);
        }

        private void mnuMark_Click(object sender, RoutedEventArgs e)
        {
            foreach (Order o in dgOrders.SelectedItems)
            {
                o.IsWorkOrder = false;
                OrderHistory h = new OrderHistory();
                h.OrderId = o.OrderId;
                h.HistoryDate = DateTime.Now;
                h.Comments = "Order marked as complete.";
                DataAccess.InsertOrderHistory(h);
                DataAccess.UpdateOrder(o);
            }
            performSearch(0);
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuMark.IsEnabled = mnuLink.IsEnabled = !(null == dgOrders.SelectedItem);
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            if (dgOrders.SelectedItem != null)
            {
                Order o = dgOrders.SelectedItem as Order;
                mw.OrderNumber = o.OrderNo;
            }
            this.Close();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                Order o = dgOrders.SelectedItem as Order;
                OrderDetails od = new OrderDetails();
                od.OrderId = o.OrderId;
                od.Owner = this;
                od.ShowDialog();
                performSearch(0);
            }
        }

        private void mnuLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Order o = dgOrders.SelectedItem as Order;
                Process.Start(o.Folder);
            }
            catch { }
        }

        private void dtDate_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Utilities.ChangeDate(sender as DatePicker, e.Delta);
            e.Handled = true;
        }
    }
}
