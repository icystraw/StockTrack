using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ItemInsight.xaml
    /// </summary>
    public partial class ItemInsight : Window
    {
        public ItemInsight()
        {
            InitializeComponent();
        }

        public int itemId = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (0 == itemId) return;
            Item i = DataAccess.GetItemById(itemId);
            if (null == i) return;
            lblItemName.Text = i.ItemName;
            lblQuantity.Content = "Quantity on hand: " + i.Quantity;
            lblQuantityW.Content = "Quantity held in work orders: " + DataAccess.QtyInWork(itemId, 1);
            lblQuantityT.Content = "Quantity quoted to tentative orders: " + DataAccess.QtyInWork(itemId, 2);
            dgItems.ItemsSource = DataAccess.GetRelatedItemsForItemInsight(itemId).DefaultView;
            dgSales.ItemsSource = DataAccess.GetItemMonthlySales(itemId).DefaultView;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        private void dgSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSales.SelectedItem != null)
            {
                double quantity = 0;
                foreach (DataRowView dr in dgSales.SelectedItems)
                {
                    quantity += Convert.ToDouble(dr["quantity"]);
                }
                lblTotalQty.Content = "Total sales quantity selected: " + quantity;
            }
        }
    }
}
