using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTrack
{
    class OrderHistory
    {
        private int orderHistoryId;

        public int OrderHistoryId
        {
            get { return orderHistoryId; }
            set { orderHistoryId = value; }
        }
        private int orderId;

        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
        private DateTime historyDate;

        public DateTime HistoryDate
        {
            get { return historyDate; }
            set { historyDate = value; }
        }
        private string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }
    }
}
