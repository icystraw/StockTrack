using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace StockTrack
{
    /// <summary>
    /// Interaction logic for Mini.xaml
    /// </summary>
    public partial class Mini : Window
    {
        public Mini()
        {
            InitializeComponent();
            this.Width = Properties.Settings.Default.miniWidth;
            this.Height = Properties.Settings.Default.miniHeight;
            this.Left = Properties.Settings.Default.miniXpos;
            this.Top = Properties.Settings.Default.miniYpos;
            this.Topmost = Properties.Settings.Default.miniTopmost;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chkTop.IsChecked = this.Topmost;
            dtEntryDate.SelectedDate = DateTime.Today;
            cbSearch.ItemsSource = DataAccess.GetAllCategories();
            cbSearch.DisplayMemberPath = "CategoryName";
            cbSearch.Focus();
            cbSearch.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(cbSearch_TextChanged));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveWindowState();
        }

        private void saveWindowState()
        {
            Properties.Settings.Default.miniWidth = (int)this.Width;
            Properties.Settings.Default.miniHeight = (int)this.Height;
            Properties.Settings.Default.miniXpos = (int)this.Left;
            Properties.Settings.Default.miniYpos = (int)this.Top;
            Properties.Settings.Default.miniTopmost = this.Topmost;
            Properties.Settings.Default.Save();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnCrazy_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem == null)
            {
                btnSave.IsEnabled = false;
                mnHistory.IsEnabled = false;
            }
            else
            {
                btnSave.IsEnabled = true;
                mnHistory.IsEnabled = true;
                populateItemHistory((dgItems.SelectedItem as Item).ItemId);
            }
        }

        private void populateItemHistory(int itemId)
        {
            dgHistory.ItemsSource = DataAccess.GetHistoryByItemId(itemId);
        }

        private void dgHistory_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
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
                h.OrderId = 0;
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
                i.Quantity += h.Quantity;
                if (h.Comments == string.Empty && h.Quantity < 0 && i.Quantity < 0)
                {
                    h.Comments = "On backorder";
                }
                DataAccess.UpdateItem(i);
                DataAccess.InsertHistory(h);
            }
            populateItemHistory((dgItems.SelectedItem as Item).ItemId);
            dgItems.Items.Refresh();
        }

        private void lblTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Maximized;
            else if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
        }

        private void txtOrderNo_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void txtQuantity_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            cbSearch.Focus();
        }

        private void chkTop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = (bool)chkTop.IsChecked;
        }

        private void cbSearch_TextChanged(object sender, RoutedEventArgs e)
        {
            dgItems.ItemsSource = DataAccess.SearchItems(cbSearch.Text);
            if (dgItems.Items.Count > 0) dgItems.SelectedIndex = 0;
        }

        private void btnShy_Click(object sender, RoutedEventArgs e)
        {
            Iconised i = new Iconised();
            i.Top = this.Top;
            i.Left = this.Left;
            i.Mini = this;
            saveWindowState();
            this.Hide();
            i.Show();
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            OrderSearch o = new OrderSearch();
            o.Show();
            this.Close();
        }

        private void dtEntryDate_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Utilities.ChangeDate(dtEntryDate, e.Delta);
        }
    }
}
