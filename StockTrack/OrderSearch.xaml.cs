using System;
using System.Collections.Generic;
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
    }
}
