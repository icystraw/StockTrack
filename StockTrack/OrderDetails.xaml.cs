using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        private void dgHistory_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            History h = e.Row.Item as History;
            DataAccess.UpdateHistoryComments(h);
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
                if (o.IsWorkOrder == 0)
                    cbIsWorkOrder.IsChecked = false;
                else if (o.IsWorkOrder == 1)
                    cbIsWorkOrder.IsChecked = true;
                else
                    cbIsWorkOrder.IsEnabled = false;
                dtOrderDate.SelectedDate = o.OrderDate;
                dtShippingDate.SelectedDate = o.ShippingDate;
                txtComments.Text = o.Comments;
                txtFolder.Text = o.Folder;
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
                if (!cbIsWorkOrder.IsEnabled)
                    o.IsWorkOrder = 2;
                else if ((bool)cbIsWorkOrder.IsChecked)
                    o.IsWorkOrder = 1;
                else
                    o.IsWorkOrder = 0;
                if (null == dtOrderDate.SelectedDate || null == dtShippingDate.SelectedDate)
                {
                    MessageBox.Show("Check.", "Please", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                o.OrderDate = (DateTime)dtOrderDate.SelectedDate;
                o.ShippingDate = (DateTime)dtShippingDate.SelectedDate;
                o.Comments = txtComments.Text.Trim();
                o.Folder = txtFolder.Text.Trim();

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
                if (o.IsWorkOrder < 2)
                    i.Quantity -= h.Quantity;
                DataAccess.DeleteHistoryById(h.HistoryId);
                if (o.IsWorkOrder < 2)
                {
                    DataAccess.UpdateItem(i);
                    foreach (Item _i in dgItems.Items)
                    {
                        if (_i.ItemId == i.ItemId) _i.Quantity = i.Quantity;
                    }
                }
            }
            dgItems.Items.Refresh();
            getOrderHistory();
        }

        private void dgHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuUndoAction.IsEnabled = mnuAddOrderComments.IsEnabled = !(null == dgHistory.SelectedItem);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getOrderDetails();
            getOrderHistory();
            getOrderProgression();
            cbSearch.ItemsSource = DataAccess.GetAllCategories();
            cbSearch.DisplayMemberPath = "CategoryName";
            cbSearch.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(cbSearch_TextChanged));
        }

        private void cbSearch_TextChanged(object sender, RoutedEventArgs e)
        {
            dgItems.ItemsSource = DataAccess.SearchItems(cbSearch.Text);
            if (dgItems.Items.Count > 0) dgItems.SelectedIndex = 0;
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgHistory.HasItems)
            {
                MessageBox.Show("As a preventative measure, please delete all the items before attempting to delete this order.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show("This will delete all related information about this order. Continue?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (o != null)
                {
                    DataAccess.DeleteOrderById(o.OrderId);
                    this.Close();
                }
            }
        }

        private void dgProgression_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            OrderHistory h = e.Row.Item as OrderHistory;
            DataAccess.UpdateOrderHistoryComments(h);
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
                txtProgress.Clear();
            }
        }

        private void dgProgression_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuDeleteEntry.IsEnabled = !(null == dgProgression.SelectedItem);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (saveOrderDetails())
                this.Close();
        }

        private void txtQuantity_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Utilities.ChangeTextFieldNumber(txtQuantity, 1);
            }
            if (e.Delta < 0)
            {
                Utilities.ChangeTextFieldNumber(txtQuantity, -1);
            }
        }

        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem == null)
            {
                btnSave.IsEnabled = false;
            }
            else
            {
                btnSave.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (null == o || dgItems.SelectedItem == null) return;

            foreach (Item i in dgItems.SelectedItems)
            {
                History h = new History();
                h.Comments = string.Empty;
                h.ItemId = i.ItemId;
                h.OrderId = orderId;
                h.OrderNo = string.Empty;
                h.EntryDate = DateTime.Now;
                try
                {
                    h.ActionDate = DateTime.Today;
                    h.Quantity = 0 - Convert.ToDouble(txtQuantity.Text.Trim());
                    if (h.Quantity == 0) return;
                }
                catch
                {
                    MessageBox.Show("Check.", "Please", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (h.Quantity <= 0)
                {
                    h.Action = "Purchase";
                }
                else
                {
                    h.Action = "Return";
                }
                if (o.IsWorkOrder < 2)
                {
                    i.Quantity += h.Quantity;
                    if (h.Quantity < 0 && i.Quantity < 0)
                    {
                        h.Comments = "On backorder";
                    }
                    DataAccess.UpdateItem(i);
                }
                else
                {
                    if (h.Quantity < 0 && i.Quantity <= 0)
                    {
                        h.Comments = "On backorder";
                    }
                }
                DataAccess.InsertHistory(h);
            }
            dgItems.Items.Refresh();
            getOrderHistory();
        }

        private void dtOrderDate_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Utilities.ChangeDate(dtOrderDate, e.Delta);
        }

        private void dtShippingDate_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Utilities.ChangeDate(dtShippingDate, e.Delta);
        }

        private void mnuAddOrderComments_Click(object sender, RoutedEventArgs e)
        {
            if (dgHistory.SelectedItem == null) return;
            foreach (History h in dgHistory.SelectedItems)
            {
                if (!string.IsNullOrEmpty(txtComments.Text.Trim()))
                    txtComments.Text += " " + h.ItemName;
                else
                    txtComments.Text += h.ItemName;
            }
        }
    }
}
