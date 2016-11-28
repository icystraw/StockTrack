﻿using System;
using System.Collections.Generic;
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
            byte? isWorkOrder = null;
            if (cbIsWorkOrder.SelectedIndex == 1) isWorkOrder = 1;
            else if (cbIsWorkOrder.SelectedIndex == 2) isWorkOrder = 2;
            SortDescription sdPrimary = new SortDescription("ShippingDate", ListSortDirection.Descending);
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
            dgOrders.ItemsSource = DataAccess.SearchOrder(txtKeyword.Text.Trim(), dtOrderDate1.SelectedDate, dtOrderDate2.SelectedDate, dtShippingDate1.SelectedDate, dtShippingDate2.SelectedDate, isWorkOrder);
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
            addNewOrder(1);
        }

        private void btnNewTentative_Click(object sender, RoutedEventArgs e)
        {
            addNewOrder(2);
        }

        private void addNewOrder(byte isWorkOrder)
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
                int orderId = 0;
                if (isWorkOrder == 1)
                    orderId = DataAccess.AddNewWorkOrder(n.Input);
                else
                    orderId = DataAccess.AddNewTentativeOrder(n.Input);
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
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                Order or = e.Row.Item as Order;
                or.Comments = tempOrder.Comments;
                or.ContactNo = tempOrder.ContactNo;
                or.CustomerName = tempOrder.CustomerName;
                or.OrderDate = tempOrder.OrderDate;
                or.PaidToday = tempOrder.PaidToday;
                or.Shipping = tempOrder.Shipping;
                or.ShippingDate = tempOrder.ShippingDate;
                or.TotalAmount = tempOrder.TotalAmount;
                return;
            }
            Order o = e.Row.Item as Order;
            DataAccess.UpdateOrder(o);
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuConvertTentative.IsEnabled = mnuConvertWorkOrder.IsEnabled = mnuMark.IsEnabled = mnuLink.IsEnabled = false;
            Order o = dgOrders.SelectedItem as Order;
            if (o != null)
            {
                mnuLink.IsEnabled = true;
                if (o.IsWorkOrder == 0)
                {
                    mnuConvertTentative.IsEnabled = true;
                    mnuConvertWorkOrder.IsEnabled = true;
                }
                if (o.IsWorkOrder == 1)
                {
                    mnuConvertTentative.IsEnabled = true;
                    mnuMark.IsEnabled = true;
                }
                if (o.IsWorkOrder == 2)
                {
                    mnuConvertWorkOrder.IsEnabled = true;
                }
            }
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

        private void cbIsWorkOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
                performSearch(0);
        }

        private void mnuMark_Click(object sender, RoutedEventArgs e)
        {
            foreach (Order o in dgOrders.SelectedItems)
            {
                if (o.IsWorkOrder != 1) continue;
                o.IsWorkOrder = 0;
                OrderHistory h = new OrderHistory();
                h.OrderId = o.OrderId;
                h.HistoryDate = DateTime.Now;
                h.Comments = "Order marked as complete.";
                DataAccess.InsertOrderHistory(h);
                DataAccess.UpdateOrder(o);
            }
            performSearch(0);
        }

        private void mnuConvertWorkOrder_Click(object sender, RoutedEventArgs e)
        {
            foreach (Order o in dgOrders.SelectedItems)
            {
                if (o.IsWorkOrder == 1) continue;
                if (o.IsWorkOrder == 2)
                {
                    List<History> hs = DataAccess.GetHistoryByOrderId(o.OrderId);
                    foreach (History h in hs)
                    {
                        Item i = DataAccess.GetItemById(h.ItemId);
                        i.Quantity += h.Quantity;
                        DataAccess.UpdateItem(i);
                    }
                }
                o.IsWorkOrder = 1;
                OrderHistory oh = new OrderHistory();
                oh.OrderId = o.OrderId;
                oh.HistoryDate = DateTime.Now;
                oh.Comments = "Order marked as work order.";
                DataAccess.InsertOrderHistory(oh);
                DataAccess.UpdateOrder(o);
            }
            performSearch(0);
        }

        private void mnuConvertTentative_Click(object sender, RoutedEventArgs e)
        {
            foreach (Order o in dgOrders.SelectedItems)
            {
                if (o.IsWorkOrder < 2)
                {
                    List<History> hs = DataAccess.GetHistoryByOrderId(o.OrderId);
                    foreach (History h in hs)
                    {
                        Item i = DataAccess.GetItemById(h.ItemId);
                        i.Quantity -= h.Quantity;
                        DataAccess.UpdateItem(i);
                    }
                    o.IsWorkOrder = 2;
                    OrderHistory oh = new OrderHistory();
                    oh.OrderId = o.OrderId;
                    oh.HistoryDate = DateTime.Now;
                    oh.Comments = "Order marked as tentative.";
                    DataAccess.InsertOrderHistory(oh);
                    DataAccess.UpdateOrder(o);
                }
            }
            performSearch(0);
        }

        private Order tempOrder = null;

        private void dgOrders_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            tempOrder = new Order();
            Order o = e.Row.Item as Order;
            tempOrder.TotalAmount = o.TotalAmount;
            tempOrder.ShippingDate = o.ShippingDate;
            tempOrder.Shipping = o.Shipping;
            tempOrder.PaidToday = o.PaidToday;
            tempOrder.OrderDate = o.OrderDate;
            tempOrder.CustomerName = o.CustomerName;
            tempOrder.ContactNo = o.ContactNo;
            tempOrder.Comments = o.Comments;
        }
    }
}
