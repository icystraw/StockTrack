using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Data;

namespace StockTrack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Width = Properties.Settings.Default.mwWidth;
            this.Height = Properties.Settings.Default.mwHeight;
            this.Left = Properties.Settings.Default.mwXpos;
            this.Top = Properties.Settings.Default.mwYpos;
        }

        public string OrderNumber
        {
            set { txtOrderNo.Text = value; }
        }

        private List<Item> itemsDisplaying = new List<Item>();
        private List<History> historyDisplaying = new List<History>();

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgCats.SelectedItem == null) return;
            Name n = new Name();
            n.Owner = this;
            n.CustomTitle = "Enter Item Name:";
            Category c = dgCats.SelectedItem as Category;
            if (n.ShowDialog() == true)
            {
                Item i = new Item();
                i.CategoryId = c.CategoryId;
                i.ItemName = n.Input;
                i.Quantity = 0;
                int newItemId = DataAccess.AddItem(i);
                if (0 == newItemId) return;
                refreshItems();
                foreach (Item _i in dgItems.Items)
                {
                    if (_i.ItemId == newItemId)
                    {
                        dgItems.SelectedItem = _i;
                        dgItems.ScrollIntoView(_i);
                        break;
                    }
                }
            }
        }

        private void btnAddCat_Click(object sender, RoutedEventArgs e)
        {
            Name n = new Name();
            n.Owner = this;
            n.CustomTitle = "Enter Category Name:";
            if (n.ShowDialog() == true)
            {
                Category c = new Category();
                c.CategoryName = n.Input;
                DataAccess.AddCategory(c);
                refreshCategories();
            }
        }

        private void dgCats_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                Category ca = e.Row.Item as Category;
                ca.CategoryName = tempCategory.CategoryName;
                return;
            }
            Category c = dgCats.SelectedItem as Category;
            DataAccess.UpdateCategory(c);
        }

        private void dgCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            relatedItemId = 0;
            if (dgCats.SelectedItem != null)
            {
                btnDeleteCat.IsEnabled = true;
                btnAddItem.IsEnabled = true;
                triggerCatSelChange = false;
                txtSearch.Text = string.Empty;
                triggerCatSelChange = true;
            }
            else
            {
                btnDeleteCat.IsEnabled = false;
                btnAddItem.IsEnabled = false;
            }
            refreshItems();
        }

        private void dgHistory_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                History hi = e.Row.Item as History;
                hi.Comments = tempHistory.Comments;
                return;
            }
            History h = e.Row.Item as History;
            DataAccess.UpdateHistoryComments(h);
        }

        private void dgItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                Item i = e.Row.Item as Item;
                i.ItemName = tempItem.ItemName;
                i.Quantity = tempItem.Quantity;
                return;
            }
            Item itemEditing = e.Row.Item as Item;
            if (e.Column.Header.ToString() == "Name")
            {
                DataAccess.UpdateItem(itemEditing);
            }
            else if (e.Column.Header.ToString() == "Quantity")
            {
                Item i = DataAccess.GetItemById(itemEditing.ItemId);
                if (null == i) return;
                double difference = itemEditing.Quantity - i.Quantity;
                if (0 == difference) return;
                History h = new History();
                h.EntryDate = DateTime.Now;
                h.ActionDate = DateTime.Now;
                h.Action = "Adjust";
                h.Comments = "New: " + itemEditing.Quantity.ToString() + " Old: " + i.Quantity.ToString();
                h.ItemId = itemEditing.ItemId;
                h.OrderNo = "ADJ" + DateTime.Today.ToString("ddMMyyyy");
                h.Quantity = difference;
                DataAccess.UpdateItem(itemEditing);
                DataAccess.InsertHistory(h);
                populateItemHistory(itemEditing.ItemId);
            }
        }

        private bool triggerCatSelChange = true;

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (triggerCatSelChange)
            {
                relatedItemId = 0;
                dgCats.SelectedItem = null;
                refreshItems();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtEntryDate.SelectedDate = DateTime.Today;
            refreshCategories();
            txtSearch.Focus();
        }

        private void refreshCategories()
        {
            List<Category> cs = DataAccess.GetAllCategories();
            dgCats.ItemsSource = cs;
            mnuMoveItem.ItemsSource = cs;
            mnuMoveItem.DisplayMemberPath = "CategoryName";
        }

        private void loadCategoryItems(int categoryId)
        {
            itemsDisplaying = DataAccess.GetAllCategoryItems(categoryId);
            dgItems.ItemsSource = itemsDisplaying;
            this.Title = dgItems.Items.Count + " item(s)";
        }

        private void loadSearchItems(string keyword)
        {
            itemsDisplaying = DataAccess.SearchItems(keyword);
            dgItems.ItemsSource = itemsDisplaying;
            this.Title = dgItems.Items.Count + " item(s)";
            if (dgItems.Items.Count > 0)
            {
                dgItems.SelectedIndex = 0;
            }
        }

        private void loadRelatedItems(int itemId)
        {
            itemsDisplaying = DataAccess.GetRelatedItems(itemId);
            dgItems.ItemsSource = itemsDisplaying;
            this.Title = dgItems.Items.Count + " item(s)";
        }

        private void btnDeleteCat_Click(object sender, RoutedEventArgs e)
        {
            if (null == dgCats.SelectedItem) return;
            if (MessageBox.Show("Sure?", "Please confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Category c = dgCats.SelectedItem as Category;
                DataAccess.DeleteCategory(c);
                refreshCategories();
            }
        }

        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem == null)
            {
                btnSave.IsEnabled = false;
                mnuDeleteItem.IsEnabled = false;
                mnuMoveItem.IsEnabled = false;
                mnuMergeItem.IsEnabled = false;
                mnuRelatedItems.IsEnabled = false;
                mnuItemInsight.IsEnabled = false;
                historyDisplaying = new List<History>();
                dgHistory.ItemsSource = historyDisplaying;
            }
            else
            {
                if (dgItems.SelectedItems.Count == 2)
                {
                    mnuMergeItem.IsEnabled = true;
                }
                else
                {
                    mnuMergeItem.IsEnabled = false;
                }
                btnSave.IsEnabled = true;
                mnuDeleteItem.IsEnabled = true;
                mnuMoveItem.IsEnabled = true;
                mnuRelatedItems.IsEnabled = true;
                mnuItemInsight.IsEnabled = true;
                if (dgItems.SelectedItems.Count == 1) populateItemHistory((dgItems.SelectedItem as Item).ItemId);
            }
        }

        private void populateItemHistory(int itemId)
        {
            historyDisplaying = DataAccess.GetHistoryByItemId(itemId);
            dgHistory.ItemsSource = historyDisplaying;
            refreshHistoryFilteredView();
        }

        private void dgHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuOrderSearch.IsEnabled = mnuUndoAction.IsEnabled = !(null == dgHistory.SelectedItem);
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            Stats s = new Stats();
            s.Show();
        }
        private int clickCount = 0;

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed) return;
            clickCount++;
            if (10 == clickCount)
            {
                btnDeleteCat.Visibility = System.Windows.Visibility.Visible;
                MessageBox.Show("This program is not meant for ordinary store with ordinary staff.\n\nIf you are in the market for a stock tracking software that makes sense, look elsewhere.\n\nDuring the making of this program, the author received valuable feedback from his colleagues and friends. They are all as crazy as - if not crazier than - the author.\n\nNo support will be provided. You are on your own.", "About");
            }
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (dgItems.SelectedItem == null) return;
            foreach (Item i in dgItems.SelectedItems)
            {
                History h = new History();
                h.Comments = string.Empty;
                h.ItemId = i.ItemId;
                if (string.IsNullOrEmpty(txtOrderNo.Text.Trim()))
                    h.OrderNo = DateTime.Today.ToString("ddMMyyyy");
                else
                    h.OrderNo = txtOrderNo.Text.Trim();
                if (dgOrders.SelectedItem != null) h.OrderId = (dgOrders.SelectedItem as Order).OrderId;
                else h.OrderId = 0;
                h.EntryDate = DateTime.Now;
                try
                {
                    h.ActionDate = (DateTime)dtEntryDate.SelectedDate;
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
                if (h.OrderId > 0)
                {
                    Order o = DataAccess.GetOrderById(h.OrderId);
                    if (null == o) return;
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
                }
                else
                {
                    i.Quantity += h.Quantity;
                    if (h.Quantity < 0 && i.Quantity < 0)
                    {
                        h.Comments = "On backorder";
                    }
                    DataAccess.UpdateItem(i);
                }
                DataAccess.InsertHistory(h);
            }
            populateItemHistory((dgItems.SelectedItem as Item).ItemId);
            dgItems.Items.Refresh();
            refreshOrder();
            txtSearch.Focus();
            txtSearch.SelectAll();
        }

        private void mnuDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem == null) return;
            Item i = dgItems.SelectedItem as Item;
            if (MessageBox.Show("Sure to delete " + i.ItemName + "?", "Please confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DataAccess.DeleteItem(i.ItemId);
                refreshItems();
            }
        }

        private int relatedItemId = 0;

        private void refreshItems()
        {
            if (relatedItemId > 0)
            {
                loadRelatedItems(relatedItemId);
            }
            else if (dgCats.SelectedItem == null)
            {
                loadSearchItems(txtSearch.Text);
            }
            else
            {
                loadCategoryItems((dgCats.SelectedItem as Category).CategoryId);
            }
        }

        private void mnuUndoAction_Click(object sender, RoutedEventArgs e)
        {
            if (dgHistory.SelectedItem == null || dgItems.SelectedItem == null) return;
            if (MessageBox.Show("Sure?", "Please confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            History h = dgHistory.SelectedItem as History;
            Order o = DataAccess.GetOrderById(h.OrderId);
            if (null == o) return;
            Item i = dgItems.SelectedItem as Item;
            if (o.IsWorkOrder < 2)
            {
                i.Quantity -= h.Quantity;
                DataAccess.UpdateItem(i);
            }
            DataAccess.DeleteHistoryById(h.HistoryId);
            populateItemHistory(i.ItemId);
            dgItems.Items.Refresh();
        }

        private void mnuMoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem == null) return;
            foreach (Item i in dgItems.SelectedItems)
            {
                Category c = (e.OriginalSource as MenuItem).Header as Category;
                i.CategoryId = c.CategoryId;
                DataAccess.UpdateItem(i);
            }
            refreshItems();
        }

        private void btnMini_Click(object sender, RoutedEventArgs e)
        {
            Mini m = new Mini();
            m.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.mwWidth = (int)this.Width;
            Properties.Settings.Default.mwHeight = (int)this.Height;
            Properties.Settings.Default.mwXpos = (int)this.Left;
            Properties.Settings.Default.mwYpos = (int)this.Top;
            Properties.Settings.Default.Save();
        }

        private void btnAdvanced_Click(object sender, RoutedEventArgs e)
        {
            Advanced a = new Advanced();
            a.Show();
        }

        private void CommandBinding_Executed_1(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            txtSearch.Focus();
            txtSearch.SelectAll();
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

        private void txtOrderNo_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Utilities.ChangeTextFieldNumber(txtOrderNo, 1);
            }
            if (e.Delta < 0)
            {
                Utilities.ChangeTextFieldNumber(txtOrderNo, -1);
            }
        }

        private void refreshHistoryFilteredView()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(dgHistory.ItemsSource);
            cv.Filter = historyDateFilter;
            cv.Refresh();
        }

        private void dtHistoryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgHistory.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(dgHistory.ItemsSource).Refresh();
            }
        }

        private bool historyDateFilter(object history)
        {
            if (null == dtHistoryDate.SelectedDate) return true;
            History h = history as History;
            if (h.ActionDate >= (DateTime)dtHistoryDate.SelectedDate && h.ActionDate < ((DateTime)dtHistoryDate.SelectedDate).AddDays(1))
                return true;
            return false;
        }

        private void mnuOrderSearch_Click(object sender, RoutedEventArgs e)
        {
            if (null == dgHistory.SelectedItem) return;
            int selectedItemId = (dgItems.SelectedItem as Item).ItemId;
            int selectedHistoryId = (dgHistory.SelectedItem as History).HistoryId;
            OrderDetails o = new OrderDetails();
            o.OrderId = (dgHistory.SelectedItem as History).OrderId;
            o.Owner = this;
            this.Hide();
            o.ShowDialog();
            refreshItems();
            foreach (Item i in dgItems.Items)
            {
                if (i.ItemId == selectedItemId)
                {
                    dgItems.SelectedItem = i;
                    break;
                }
            }
            foreach (History h in dgHistory.Items)
            {
                if (h.HistoryId == selectedHistoryId)
                {
                    dgHistory.SelectedItem = h;
                    break;
                }
            }
            refreshOrder();
            dgHistory.Focus();
        }

        private void refreshOrder()
        {
            int selectedOrderId = 0;
            if (dgOrders.SelectedItem != null)
            {
                selectedOrderId = (dgOrders.SelectedItem as Order).OrderId;
            }
            if (!string.IsNullOrEmpty(txtOrderNo.Text.Trim()))
            {
                dgOrders.ItemsSource = DataAccess.GetOrderByNo(txtOrderNo.Text.Trim());
            }
            else
            {
                dgOrders.ItemsSource = new List<Order>();
            }
            if (selectedOrderId > 0 && dgOrders.HasItems)
            {
                foreach (Order o in dgOrders.Items)
                {
                    if (o.OrderId == selectedOrderId)
                    {
                        dgOrders.SelectedItem = o;
                        break;
                    }
                }
            }
            else if (dgOrders.HasItems)
            {
                dgOrders.SelectedIndex = 0;
            }
        }

        private void txtOrderNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshOrder();
        }

        private void dgOrders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (null == dgOrders.SelectedItem) return;
            int selectedItemId = 0;
            int selectedHistoryId = 0;
            if (dgItems.SelectedItem != null)
            {
                selectedItemId = (dgItems.SelectedItem as Item).ItemId;
            }
            if (dgHistory.SelectedItem != null)
            {
                selectedHistoryId = (dgHistory.SelectedItem as History).HistoryId;
            }
            OrderDetails o = new OrderDetails();
            o.OrderId = (dgOrders.SelectedItem as Order).OrderId;
            o.Owner = this;
            this.Hide();
            o.ShowDialog();
            refreshItems();
            if (selectedItemId > 0)
            {
                foreach (Item i in dgItems.Items)
                {
                    if (i.ItemId == selectedItemId)
                    {
                        dgItems.SelectedItem = i;
                        break;
                    }
                }
            }
            if (selectedHistoryId > 0)
            {
                foreach (History h in dgHistory.Items)
                {
                    if (h.HistoryId == selectedHistoryId)
                    {
                        dgHistory.SelectedItem = h;
                        break;
                    }
                }
            }
            refreshOrder();
        }

        private void btnGoOrders_Click(object sender, RoutedEventArgs e)
        {
            OrderSearch os = new OrderSearch();
            if (dgItems.SelectedItem != null)
            {
                os.searchKeyword = (dgItems.SelectedItem as Item).ItemName;
            }
            os.Show();
            this.Close();
        }

        private void dtEntryDate_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Utilities.ChangeDate(dtEntryDate, e.Delta);
        }

        private void dtHistoryDate_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Utilities.ChangeDate(dtHistoryDate, e.Delta);
        }

        private void mnuMergeItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItems.Count != 2) return;
            MergeItems mi = new MergeItems();
            mi.Owner = this;
            mi.ItemList = new List<Item>();
            foreach (Item i in dgItems.SelectedItems)
                mi.ItemList.Add(i);
            if (mi.ShowDialog() == true)
            {
                refreshItems();
            }
        }

        private Item tempItem = null;

        private void dgItems_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            tempItem = new Item();
            Item i = e.Row.Item as Item;
            tempItem.ItemName = i.ItemName;
            tempItem.Quantity = i.Quantity;
        }

        private History tempHistory = null;

        private void dgHistory_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            tempHistory = new History();
            History h = e.Row.Item as History;
            tempHistory.Comments = h.Comments;
        }

        private Category tempCategory = null;

        private void dgCats_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            tempCategory = new Category();
            Category c = e.Row.Item as Category;
            tempCategory.CategoryName = c.CategoryName;
        }

        private void mnuRelatedItems_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem != null)
            {
                Item i = dgItems.SelectedItem as Item;
                relatedItemId = i.ItemId;
                refreshItems();
            }
        }

        private void chkTopMost_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        private void chkTopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        private void mnuItemInsight_Click(object sender, RoutedEventArgs e)
        {
            ItemInsight ii = new ItemInsight();
            ii.itemId = (dgItems.SelectedItem as Item).ItemId;
            ii.Owner = this;
            ii.Show();
        }
    }
}
