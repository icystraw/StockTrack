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
            lblItemName.Content = i.ItemName;
            lblQuantity.Content = "Quantity on hand: " + i.Quantity;
            dgItems.ItemsSource = DataAccess.GetRelatedItemsForItemInsight(itemId).DefaultView;
            dgSales.ItemsSource = DataAccess.GetItemMonthlySales(itemId).DefaultView;
        }
    }
}
