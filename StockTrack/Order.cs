using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StockTrack
{
    class Order : INotifyPropertyChanged
    {
        private int orderId;

        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
        private string orderNo;

        public string OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }
        private string customerName;

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; OnPropertyChanged("CustomerName"); }
        }
        private string contactNo;

        public string ContactNo
        {
            get { return contactNo; }
            set { contactNo = value; OnPropertyChanged("ContactNo"); }
        }
        private string shipping;

        public string Shipping
        {
            get { return shipping; }
            set { shipping = value; OnPropertyChanged("Shipping"); }
        }
        private double totalAmount;

        public double TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; OnPropertyChanged("TotalAmount"); OnPropertyChanged("Balance"); }
        }
        private double paidToday;

        public double PaidToday
        {
            get { return paidToday; }
            set { paidToday = value; OnPropertyChanged("PaidToday"); OnPropertyChanged("Balance"); }
        }
        private byte isWorkOrder;

        public byte IsWorkOrder
        {
            get { return isWorkOrder; }
            set { isWorkOrder = value; }
        }
        private DateTime orderDate;

        public DateTime OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; OnPropertyChanged("OrderDate"); }
        }
        private DateTime shippingDate;

        public DateTime ShippingDate
        {
            get { return shippingDate; }
            set { shippingDate = value; OnPropertyChanged("ShippingDate"); OnPropertyChanged("ShippingDateColor"); }
        }
        private string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; OnPropertyChanged("Comments"); }
        }
        private string folder;

        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }
        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string progress;

        public string Progress
        {
            get { return progress; }
            set { progress = value; }
        }

        public double Balance
        {
            get { return totalAmount - paidToday; }
        }

        public Brush ShippingDateColor
        {
            get
            {
                if (this.shippingDate >= DateTime.Today && this.shippingDate < DateTime.Today.AddDays(1))
                    return new SolidColorBrush(Colors.Pink);
                else if (this.shippingDate >= DateTime.Today.AddDays(-6) && this.shippingDate <= DateTime.Today.AddDays(6))
                    return new SolidColorBrush(Colors.PowderBlue);
                return new SolidColorBrush(Colors.White);
            }
        }

        public Brush OrderColor
        {
            get
            {
                if (isWorkOrder == 1)
                    return new SolidColorBrush(Colors.Pink);
                else if (isWorkOrder == 2)
                    return new SolidColorBrush(Colors.LightGray);
                return new SolidColorBrush(Colors.White);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
