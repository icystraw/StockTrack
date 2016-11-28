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
    /// Interaction logic for MergeItems.xaml
    /// </summary>
    public partial class MergeItems : Window
    {
        public MergeItems()
        {
            InitializeComponent();
        }

        public List<Item> ItemList;

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtConfirm.Text != "merge") return;
            this.DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbItems.ItemsSource = ItemList;
            cbItems.DisplayMemberPath = "ItemName";
        }
    }
}
