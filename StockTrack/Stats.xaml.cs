using Microsoft.Win32;
using OfficeOpenXml;
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

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!dgHistory.HasItems) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Microsoft Excel Workbook|*.xlsx";
            saveFileDialog1.Title = "Export to";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != string.Empty)
            {
                List<History> hs = dgHistory.ItemsSource as List<History>;
                ExcelPackage p = new ExcelPackage();
                p.File = new System.IO.FileInfo(saveFileDialog1.FileName);
                p.Workbook.Worksheets.Add("Report");
                for (int i = 0; i < hs.Count; i++)
                {
                    p.Workbook.Worksheets["Report"].Cells[i + 1, 1].Value = hs[i].ItemName;
                    p.Workbook.Worksheets["Report"].Cells[i + 1, 2].Value = hs[i].Quantity;
                }
                p.Save();
            }
        }
    }
}
