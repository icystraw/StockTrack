﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StockTrack
{
    class Order
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
            set { customerName = value; }
        }
        private string contactNo;

        public string ContactNo
        {
            get { return contactNo; }
            set { contactNo = value; }
        }
        private string shipping;

        public string Shipping
        {
            get { return shipping; }
            set { shipping = value; }
        }
        private double totalAmount;

        public double TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }
        private double paidToday;

        public double PaidToday
        {
            get { return paidToday; }
            set { paidToday = value; }
        }
        private bool isWorkOrder;

        public bool IsWorkOrder
        {
            get { return isWorkOrder; }
            set { isWorkOrder = value; }
        }
        private DateTime orderDate;

        public DateTime OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }
        private DateTime shippingDate;

        public DateTime ShippingDate
        {
            get { return shippingDate; }
            set { shippingDate = value; }
        }
        private string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        private string folder;

        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        private string latestProgress;

        public string LatestProgress
        {
            get { return latestProgress; }
            set { latestProgress = value; }
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
                    return new SolidColorBrush(Colors.PaleGreen);
                return new SolidColorBrush(Colors.White);
            }
        }
    }
}
