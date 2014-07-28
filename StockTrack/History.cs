using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StockTrack
{
    class History
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
            set { action = value; }
        }
        private double quantity;

        public double Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        private string orderNo;

        public string OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }
        private bool isWorkOrder;

        public bool IsWorkOrder
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

        public Brush OrderColor
        {
            get
            {
                if (isWorkOrder)
                    return new SolidColorBrush(Colors.Pink);
                return new SolidColorBrush(Colors.White);
            }
        }
    }
}
