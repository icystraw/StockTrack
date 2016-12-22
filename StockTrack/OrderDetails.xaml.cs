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

        private List<int> orderCandidates = null;

        public List<int> OrderCandidates
        {
            set { this.orderCandidates = value; }
        }

        private bool formResult = false;

        public bool FormResult
        {
            get { return formResult; }
        }

        private List<History> hs = new List<History>();
        private List<OrderHistory> ohs = new List<OrderHistory>();
        private Order o = null;
        private DateTime nullDate = new DateTime(1970, 1, 1);

        private void dgHistory_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                History hi = e.Row.Item as History;
                hi.Comments = tempHistory.Comments;
                hi.Quantity = tempHistory.Quantity;
                return;
            }
            History h = e.Row.Item as History;
            if (h.Quantity != 0)
            {
                if (h.Action != "Adjust")
                {
                    if (h.Quantity <= 0) h.Action = "Purchase";
                    else h.Action = "Return";
                }
                DataAccess.UpdateHistory(h);
                double qtyChange = h.Quantity - tempHistory.Quantity;
                if (qtyChange != 0)
                {
                    Item i = DataAccess.GetItemById(h.ItemId);
                    i.Quantity += qtyChange;
                    DataAccess.UpdateItem(i);
                    foreach (Item _i in dgItems.Items)
                    {
                        if (_i.ItemId == i.ItemId)
                        {
                            _i.Quantity = i.Quantity;
                            break;
                        }
                    }
                }
            }
            else
            {
                h.Quantity = tempHistory.Quantity;
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
                cbIsWorkOrder.IsEnabled = true;
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
                txtEmail.Text = o.Email;
                txtAddress.Text = o.Address;
                dataChanged = false;
            }
        }

        private bool saveOrderDetails()
        {
            if (o != null && dataChanged)
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
                o.Email = txtEmail.Text.Trim();
                o.Address = txtAddress.Text.Trim();

                DataAccess.UpdateOrder(o);
                dataChanged = false;
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
                        if (_i.ItemId == i.ItemId)
                        {
                            _i.Quantity = i.Quantity;
                            break;
                        }
                    }
                }
            }
            getOrderHistory();
        }

        private void dgHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuEmailOrder.IsEnabled = mnuUndoAction.IsEnabled = mnuAddOrderComments.IsEnabled = !(null == dgHistory.SelectedItem);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            prepareOrder();
            cbSearch.ItemsSource = DataAccess.GetAllCategories();
            cbSearch.DisplayMemberPath = "CategoryName";
            cbSearch.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(cbSearch_TextChanged));
            cbShipping.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(cbShipping_TextChanged));
        }

        private void prepareOrder()
        {
            getOrderDetails();
            getOrderHistory();
            getOrderProgression();
        }

        private void cbSearch_TextChanged(object sender, RoutedEventArgs e)
        {
            dgItems.ItemsSource = DataAccess.SearchItems(cbSearch.Text);
            if (dgItems.Items.Count > 0) dgItems.SelectedIndex = 0;
            btnQuickAdd.IsEnabled = !string.IsNullOrEmpty(cbSearch.Text.Trim());
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
                    dataChanged = false;
                    formResult = true;
                    this.Close();
                }
            }
        }

        private void dgProgression_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                OrderHistory o = e.Row.Item as OrderHistory;
                o.Comments = tempOrderHistory.Comments;
                return;
            }
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
                this.formResult = true;
            }
        }

        private void dgProgression_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuDeleteEntry.IsEnabled = !(null == dgProgression.SelectedItem);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (saveOrderDetails())
            {
                this.formResult = true;
                this.Close();
            }
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

        private OrderHistory tempOrderHistory = null;

        private void dgProgression_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            tempOrderHistory = new OrderHistory();
            tempOrderHistory.Comments = (e.Row.Item as OrderHistory).Comments;
        }

        private History tempHistory = null;

        private void dgHistory_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            tempHistory = new History();
            tempHistory.Comments = (e.Row.Item as History).Comments;
            tempHistory.Quantity = (e.Row.Item as History).Quantity;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
                this.Owner.Topmost = true;
                this.Owner.Topmost = false;
            }
        }

        private void btnQuickAdd_Click(object sender, RoutedEventArgs e)
        {
            CategorySelection cs = new CategorySelection();
            cs.Owner = this;
            if (true == cs.ShowDialog())
            {
                Item i = new Item();
                i.ItemName = cbSearch.Text.Trim();
                i.Quantity = 0;
                i.CategoryId = cs.CategoryId;
                DataAccess.AddItem(i);
                cbSearch_TextChanged(this, null);
            }
        }

        private bool dataChanged = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !canMoveOn();
        }

        private bool canMoveOn()
        {
            if (dataChanged || txtProgress.Text != string.Empty)
            {
                if (MessageBox.Show("Data Changed. Continue?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private void txtOrderNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void dtOrderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtContactNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtTotalAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtPaidToday_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void dtShippingDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtComments_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void cbShipping_TextChanged(object sender, RoutedEventArgs e)
        {
            dataChanged = true;
        }

        private void cbIsWorkOrder_Checked(object sender, RoutedEventArgs e)
        {
            dataChanged = true;
        }

        private void cbIsWorkOrder_Unchecked(object sender, RoutedEventArgs e)
        {
            dataChanged = true;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && !(e.OriginalSource is DataGridCell))
            {
                this.Close();
            }
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            this.formResult = saveOrderDetails();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (null == this.orderCandidates) return;
            if (!canMoveOn()) return;
            int position = 0;
            for (int i = 0; i < orderCandidates.Count; i++)
            {
                if (orderCandidates[i] == this.orderId)
                {
                    position = i;
                    break;
                }
            }
            if (position == 0) return;
            this.orderId = orderCandidates[position - 1];
            prepareOrder();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (null == this.orderCandidates) return;
            if (!canMoveOn()) return;
            int position = 0;
            for (int i = 0; i < orderCandidates.Count; i++)
            {
                if (orderCandidates[i] == this.orderId)
                {
                    position = i;
                    break;
                }
            }
            if (position == orderCandidates.Count - 1) return;
            this.orderId = orderCandidates[position + 1];
            prepareOrder();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void txtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:" + txtEmail.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMapAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.google.com.au/maps/search/" + System.Web.HttpUtility.UrlEncode(txtAddress.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuEmailOrder_Click(object sender, RoutedEventArgs e)
        {
            Utilities.EmailOrder((dgHistory.SelectedItem as History).ItemName, 0 - (dgHistory.SelectedItem as History).Quantity, o.OrderNo);
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            btnAddProgress_Click(this, null);
        }

        private void CommandBinding_Executed_1(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (btnSave.IsEnabled)
            {
                btnSave_Click(this, null);
            }
        }
    }
}
