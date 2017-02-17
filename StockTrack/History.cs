using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StockTrack
{
    class History : INotifyPropertyChanged
    {
        private int historyId;

        public int HistoryId
        {
            get { return historyId; }
            set { historyId = value; }
        }
        private int itemId;

        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }
        private string itemName;

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        private DateTime entryDate;

        public DateTime EntryDate
        {
            get { return entryDate; }
            set { entryDate = value; }
        }
        private DateTime actionDate;

        public DateTime ActionDate
        {
            get { return actionDate; }
            set { actionDate = value; }
        }
        private string action;

        public string Action
        {
            get { return action; }
            set { action = value; OnPropertyChanged("Action"); }
        }
        private double quantity;

        public double Quantity
        {
            get { return quantity; }
            set { quantity = value; OnPropertyChanged("Quantity"); OnPropertyChanged("Action"); }
        }
        private double itemQuantity;

        public double ItemQuantity
        {
            get { return itemQuantity; }
            set { itemQuantity = value; }
        }
        private string orderNo;

        public string OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }
        private byte isWorkOrder;

        public byte IsWorkOrder
        {
            get { return isWorkOrder; }
            set { isWorkOrder = value; }
        }
        private int orderId;

        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        private string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        private string customerName;

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        private string contactNo;

        public string ContactNo
        {
            get { return contactNo; }
            set { contactNo = value; }
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
