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
    /// Interaction logic for CategorySelection.xaml
    /// </summary>
    public partial class CategorySelection : Window
    {
        public CategorySelection()
        {
            InitializeComponent();
        }

        public int CategoryId = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Category> cs = DataAccess.GetAllCategories();
            dgCats.ItemsSource = cs;
        }

        private void dgCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOK.IsEnabled = !(dgCats.SelectedItem == null);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (dgCats.SelectedItem != null)
            {
                CategoryId = (dgCats.SelectedItem as Category).CategoryId;
            }
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
