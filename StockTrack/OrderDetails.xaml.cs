using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace StockTrack
{
    /// <summary>
    /// Interaction logic for OrderDetails.xaml
    /// </summary>
    public partial class OrderDetails : Window
    {
        public OrderDetails()
        {
            InitializeComponent();
        }
        private int orderId;

        public int OrderId
        {
            set
            {
                orderId = value;
            }
            get
            {
                return orderId;
            }
        }

        private List<History> hs = new List<History>();
        private List<OrderHistory> ohs = new List<OrderHistory>();
        private Order o = null;
        private DateTime nullDate = new DateTime(1970, 1, 1);
        private bool needSaveOrder = true;

        private void dgHistory_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                string newComments = (e.EditingElement as TextBox).Text.Trim();
                History h = e.Row.Item as History;
                h.Comments = newComments;
                DataAccess.UpdateHistoryComments(h);
            }
        }

        private void getOrderDetails()
        {
            o = DataAccess.GetOrderById(orderId);
            if (o != null)
            {
                txtOrderNo.Text = o.OrderNo;
                txtCustomerName.Text = o.CustomerName;
                txtContactNo.Text = o.ContactNo;
                cbShipping.Text = o.Shipping;
                txtTotalAmount.Text = o.TotalAmount.ToString();
                txtPaidToday.Text = o.PaidToday.ToString();
                cbIsWorkOrder.IsChecked = o.IsWorkOrder;
                dtOrderDate.SelectedDate = o.OrderDate;
                dtShippingDate.SelectedDate = o.ShippingDate;
                txtComments.Text = o.Comments;
            }
        }

        private bool saveOrderDetails()
        {
            if (o != null)
            {
                o.OrderNo = txtOrderNo.Text.Trim();
                o.CustomerName = txtCustomerName.Text.Trim();
                o.ContactNo = txtContactNo.Text.Trim();
                o.Shipping = cbShipping.Text.Trim();
                try
                {
                    o.TotalAmount = Convert.ToDouble(txtTotalAmount.Text.Trim());
                    o.PaidToday = Convert.ToDouble(txtPaidToday.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Check.", "Please", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                o.IsWorkOrder = (bool)cbIsWorkOrder.IsChecked;
                if (null == dtOrderDate.SelectedDate || null == dtShippingDate.SelectedDate)
                {
                    MessageBox.Show("Check.", "Please", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                o.OrderDate = (DateTime)dtOrderDate.SelectedDate;
                o.ShippingDate = (DateTime)dtShippingDate.SelectedDate;
                o.Comments = txtComments.Text.Trim();

                DataAccess.UpdateOrder(o);
            }
            return true;
        }

        private void getOrderHistory()
        {
            hs = DataAccess.GetHistoryByOrderId(orderId);
            dgHistory.ItemsSource = hs;
        }

        private void getOrderProgression()
        {
            ohs = DataAccess.GetOrderHistoryByOrderId(orderId);
            dgProgression.ItemsSource = ohs;
        }

        private void mnuUndoAction_Click(object sender, RoutedEventArgs e)
        {
            if (dgHistory.SelectedItem == null) return;
            if (MessageBox.Show("Sure?", "Please confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            foreach (History h in dgHistory.SelectedItems)
            {
                Item i = DataAccess.GetItemById(h.ItemId);
                if (null == i) return;
                i.Quantity -= h.Quantity;
                DataAccess.DeleteHistoryById(h.HistoryId);
                DataAccess.UpdateItem(i);
            }
            getOrderHistory();
        }

        private void dgHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuUndoAction.IsEnabled = !(null == dgHistory.SelectedItem);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getOrderDetails();
            getOrderHistory();
            getOrderProgression();
        }

        private void amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtBalance.Text = (Convert.ToDouble(txtTotalAmount.Text.Trim()) - Convert.ToDouble(txtPaidToday.Text.Trim())).ToString();
            }
            catch
            {
                txtBalance.Text = string.Empty;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (needSaveOrder)
            {
                e.Cancel = !saveOrderDetails();
            }
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgHistory.HasItems)
            {
                MessageBox.Show("As a preventative measure, please undo all the items before attempting to delete this order.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show("This will delete all related information about this order. Continue?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (o != null)
                {
                    DataAccess.DeleteOrderById(o.OrderId);
                    needSaveOrder = false;
                    this.Close();
                }
            }
        }

        private void dgProgression_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                string newComments = (e.EditingElement as TextBox).Text.Trim();
                OrderHistory h = e.Row.Item as OrderHistory;
                h.Comments = newComments;
                DataAccess.UpdateOrderHistoryComments(h);
            }
        }

        private void mnuDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            foreach (OrderHistory h in dgProgression.SelectedItems)
            {
                DataAccess.DeleteOrderHistoryById(h.OrderHistoryId);
            }
            getOrderProgression();
        }

        private void btnAddProgress_Click(object sender, RoutedEventArgs e)
        {
            if (o != null)
            {
                OrderHistory h = new OrderHistory();
                h.OrderId = orderId;
                h.HistoryDate = DateTime.Now;
                h.Comments = txtProgress.Text.Trim();
                if (string.IsNullOrEmpty(h.Comments)) return;
                DataAccess.InsertOrderHistory(h);
                getOrderProgression();
            }
        }

        private void dgProgression_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuDeleteEntry.IsEnabled = !(null == dgProgression.SelectedItem);
        }
    }
}
