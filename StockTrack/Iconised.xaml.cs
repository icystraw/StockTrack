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
    /// Interaction logic for Iconised.xaml
    /// </summary>
    public partial class Iconised : Window
    {
        public Iconised()
        {
            InitializeComponent();
        }

        private Window mini = null;

        public Window Mini
        {
            get { return mini; }
            set { mini = value; }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Opacity = 0.9;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Opacity = 0.5;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mini != null)
            {
                mini.Top = this.Top;
                mini.Left = this.Left;
                mini.Show();
                this.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void mnExit_Click(object sender, RoutedEventArgs e)
        {
            if (mini != null)
            {
                mini.Close();
                this.Close();
            }
        }

        private void mnOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderSearch o = new OrderSearch();
            o.Show();
            this.Close();
            if (mini != null)
            {
                mini.Close();
            }
        }

        private void mnItems_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
            if (mini != null)
            {
                mini.Close();
            }
        }
    }
}
