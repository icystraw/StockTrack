using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace StockTrack
{
    /// <summary>
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : Window
    {
        public Stats()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            dgHistory.ItemsSource = DataAccess.GetHistoricSales((DateTime)dt1.SelectedDate, (DateTime)dt2.SelectedDate, cbCategories.SelectedItem as Category);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt1.SelectedDate = DateTime.Today.AddDays(-90);
            dt2.SelectedDate = DateTime.Today.AddDays(1);
            List<Category> cats = DataAccess.GetAllCategories();
            Category allCats = new Category();
            allCats.CategoryId = 0;
            allCats.CategoryName = "All";
            cats.Insert(0, allCats);
            cbCategories.ItemsSource = cats;
            cbCategories.DisplayMemberPath = "CategoryName";
            cbCategories.SelectedIndex = 0;
        }
    }
}
