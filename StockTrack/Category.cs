﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTrack
{
    class Category
    {
        private int categoryId;

        public int CategoryId
        {
            get { return categoryId; }
            set { categoryId = value; }
        }
        private string categoryName;

        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value; }
        }
    }
}
