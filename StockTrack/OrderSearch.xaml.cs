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
        }

        public string OrderNumber
        {
            set
            {
                txtKeyword.Text = value.Trim();
            }
        }

        private MainWindow mainWindow;

        public MainWindow MainWindow
        {
            set
            {
                mainWindow = value;
            }
        }

        List<History> hs = new List<History>();

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

        public void PerformSearch()
        {
            if (txtKeyword.Text.Trim() != string.Empty)
            {
                hs = DataAccess.GetHistoryByOrderNo(txtKeyword.Text.Trim());
                dgHistory.ItemsSource = hs;
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
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
            PerformSearch();
        }

        private void dgHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mnuUndoAction.IsEnabled = !(null == dgHistory.SelectedItem);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.RefreshItems();
            mainWindow.Show();
        }
    }
}
