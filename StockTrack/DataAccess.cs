using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace StockTrack
{
    class DataAccess
    {
        private static string conStr = ConfigurationManager.ConnectionStrings["constr"].ToString();

        public static List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select * from [category] order by [categoryname]", con);
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                Category c = new Category();
                c.CategoryId = Convert.ToInt32(rd["categoryid"]);
                c.CategoryName = rd["categoryname"].ToString();
                categories.Add(c);
            }
            rd.Close();

            return categories;
        }

        public static void UpdateCategory(Category c)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [category] set [categoryname] = @categoryname where [categoryid] = @categoryid", con);
            con.Open();
            cmd.Parameters.Add(new SqlParameter("@categoryid", c.CategoryId));
            cmd.Parameters.Add(new SqlParameter("@categoryname", c.CategoryName));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void AddCategory(Category c)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("insert into [category] ([categoryname]) values (@categoryname)", con);
            con.Open();
            cmd.Parameters.Add(new SqlParameter("@categoryname", c.CategoryName));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteCategory(Category c)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("delete from [category] where [categoryid] = @categoryid", con);
            con.Open();
            cmd.Parameters.Add(new SqlParameter("@categoryid", c.CategoryId));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<Item> GetAllCategoryItems(int categoryId)
        {
            List<Item> items = new List<Item>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select * from [item] where [categoryid] = @categoryid order by [itemname]", con);
            cmd.Parameters.Add(new SqlParameter("@categoryid", categoryId));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                Item i = new Item();
                i.ItemId = Convert.ToInt32(rd["itemid"]);
                i.CategoryId = Convert.ToInt32(rd["categoryid"]);
                i.Quantity = Convert.ToDouble(rd["quantity"]);
                i.ItemName = rd["itemname"].ToString();
                items.Add(i);
            }
            rd.Close();

            return items;
        }

        public static List<Item> SearchItems(string keyword)
        {
            List<Item> items = new List<Item>();
            if (string.IsNullOrEmpty(keyword)) return items;
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select [item].* from [item] inner join [category] on [category].[categoryid] = [item].[categoryid] where [item].[itemname] like N'%' + @itemname + N'%' or [category].[categoryname] like N'%' + @itemname + N'%' order by [item].[itemname]", con);
            cmd.Parameters.Add(new SqlParameter("@itemname", keyword));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                Item i = new Item();
                i.ItemId = Convert.ToInt32(rd["itemid"]);
                i.CategoryId = Convert.ToInt32(rd["categoryid"]);
                i.Quantity = Convert.ToDouble(rd["quantity"]);
                i.ItemName = rd["itemname"].ToString();
                items.Add(i);
            }
            rd.Close();

            return items;
        }

        public static void AddItem(Item i)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("insert into [item] ([categoryid], [itemname], [quantity]) values (@categoryid, @itemname, 0)", con);
            con.Open();
            cmd.Parameters.Add(new SqlParameter("@categoryid", i.CategoryId));
            cmd.Parameters.Add(new SqlParameter("@itemname", i.ItemName));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteItem(int itemId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("delete from [item] where [itemid] = @itemid", con);
            con.Open();
            cmd.Parameters.Add(new SqlParameter("@itemid", itemId));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static Item GetItemById(int itemId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select * from [item] where [itemid] = @itemid", con);
            cmd.Parameters.Add(new SqlParameter("@itemid", itemId));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            Item i = null;
            if (rd.Read())
            {
                i = new Item();
                i.ItemId = Convert.ToInt32(rd["itemid"]);
                i.CategoryId = Convert.ToInt32(rd["categoryid"]);
                i.Quantity = Convert.ToDouble(rd["quantity"]);
                i.ItemName = rd["itemname"].ToString();
            }
            rd.Close();

            return i;
        }

        public static void UpdateItem(Item i)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [item] set [categoryid] = @categoryid, [itemname] = @itemname, [quantity] = @quantity where [itemid] = @itemid", con);
            cmd.Parameters.Add(new SqlParameter("@itemid", i.ItemId));
            cmd.Parameters.Add(new SqlParameter("@categoryid", i.CategoryId));
            cmd.Parameters.Add(new SqlParameter("@itemname", i.ItemName));
            cmd.Parameters.Add(new SqlParameter("@quantity", i.Quantity));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void InsertHistory(History h)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("inserthistory", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@itemid", h.ItemId));
            cmd.Parameters.Add(new SqlParameter("@entrydate", h.EntryDate));
            cmd.Parameters.Add(new SqlParameter("@actiondate", h.ActionDate));
            cmd.Parameters.Add(new SqlParameter("@action", h.Action));
            cmd.Parameters.Add(new SqlParameter("@quantity", h.Quantity));
            cmd.Parameters.Add(new SqlParameter("@orderno", h.OrderNo));
            cmd.Parameters.Add(new SqlParameter("@comments", h.Comments));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void UpdateHistoryComments(History h)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [history] set [comments] = @comments where [historyid] = @historyid", con);
            cmd.Parameters.Add(new SqlParameter("@comments", h.Comments));
            cmd.Parameters.Add(new SqlParameter("@historyid", h.HistoryId));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteHistoryById(int historyId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("delete from [history] where [historyid] = @historyid", con);
            cmd.Parameters.Add(new SqlParameter("@historyid", historyId));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<History> GetHistoryByItemId(int itemId)
        {
            List<History> histories = new List<History>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select [history].*, [order].[orderno] from [history] inner join [order] on [order].[orderid] = [history].[orderid] where [itemid] = @itemid order by [entrydate] desc", con);
            cmd.Parameters.Add(new SqlParameter("@itemid", itemId));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                History h = new History();
                h.HistoryId = Convert.ToInt32(rd["historyid"]);
                h.ItemId = Convert.ToInt32(rd["itemid"]);
                h.EntryDate = Convert.ToDateTime(rd["entrydate"]);
                h.ActionDate = Convert.ToDateTime(rd["actiondate"]);
                h.Action = rd["action"].ToString();
                h.Comments = rd["comments"].ToString();
                h.OrderNo = rd["orderno"].ToString();
                h.OrderId = Convert.ToInt32(rd["orderid"]);
                h.Quantity = Convert.ToDouble(rd["quantity"]);
                histories.Add(h);
            }
            rd.Close();

            return histories;
        }

        public static List<History> GetHistoryByOrderId(int orderId)
        {
            List<History> histories = new List<History>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select [history].*, [item].[itemname], [order].[orderno] from [history] inner join [item] on [item].[itemid] = [history].[itemid] inner join [order] on [order].[orderid] = [history].[orderid] where [history].[orderid] = @orderid order by [entrydate] desc", con);
            cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                History h = new History();
                h.HistoryId = Convert.ToInt32(rd["historyid"]);
                h.ItemId = Convert.ToInt32(rd["itemid"]);
                h.ItemName = rd["itemname"].ToString();
                h.EntryDate = Convert.ToDateTime(rd["entrydate"]);
                h.ActionDate = Convert.ToDateTime(rd["actiondate"]);
                h.Action = rd["action"].ToString();
                h.Comments = rd["comments"].ToString();
                h.OrderId = Convert.ToInt32(rd["orderid"]);
                h.OrderNo = rd["orderno"].ToString();
                h.Quantity = Convert.ToDouble(rd["quantity"]);
                histories.Add(h);
            }
            rd.Close();

            return histories;
        }

        public static Order GetOrderById(int orderId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select * from [order] where [orderid] = @orderid", con);
            cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            Order o = null;
            if (rd.Read())
            {
                o = new Order();
                o.OrderId = Convert.ToInt32(rd["orderid"]);
                o.OrderNo = rd["orderno"].ToString();
                o.CustomerName = rd["customername"].ToString();
                o.ContactNo = rd["contactno"].ToString();
                o.Shipping = rd["shipping"].ToString();
                o.TotalAmount = Convert.ToDouble(rd["totalamount"]);
                o.PaidToday = Convert.ToDouble(rd["paidtoday"]);
                o.IsWorkOrder = Convert.ToBoolean(rd["isworkorder"]);
                o.OrderDate = Convert.ToDateTime(rd["orderdate"]);
                o.ShippingDate = Convert.ToDateTime(rd["shippingdate"]);
                o.Comments = rd["comments"].ToString();
            }
            rd.Close();

            return o;
        }

        public static List<Order> SearchOrder(string orderNo, string keyword, string shipping, DateTime? startDate, DateTime? endDate, bool? isWorkOrder)
        {
            List<Order> orders = new List<Order>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "select * from [order] where 1 = 1";


            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                Order o = new Order();
                o.OrderId = Convert.ToInt32(rd["orderid"]);
                o.OrderNo = rd["orderno"].ToString();
                o.CustomerName = rd["customername"].ToString();
                o.ContactNo = rd["contactno"].ToString();
                o.Shipping = rd["shipping"].ToString();
                o.TotalAmount = Convert.ToDouble(rd["totalamount"]);
                o.PaidToday = Convert.ToDouble(rd["paidtoday"]);
                o.IsWorkOrder = Convert.ToBoolean(rd["isworkorder"]);
                o.OrderDate = Convert.ToDateTime(rd["orderdate"]);
                o.ShippingDate = Convert.ToDateTime(rd["shippingdate"]);
                o.Comments = rd["comments"].ToString();
                orders.Add(o);
            }
            rd.Close();

            return orders;
        }

        public static void UpdateOrder(Order o)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [order] set [orderno] = @orderno, [customername] = @customername, [contactno] = @contactno, [shipping] = @shipping, [totalamount] = @totalamount, [paidtoday] = @paidtoday, [isworkorder] = @isworkorder, [orderdate] = @orderdate, [shippingdate] = @shippingdate, [comments] = @comments where [orderid] = @orderid", con);
            cmd.Parameters.Add(new SqlParameter("@orderno", o.OrderNo));
            cmd.Parameters.Add(new SqlParameter("@customername", o.CustomerName));
            cmd.Parameters.Add(new SqlParameter("@contactno", o.ContactNo));
            cmd.Parameters.Add(new SqlParameter("@shipping", o.Shipping));
            cmd.Parameters.Add(new SqlParameter("@totalamount", o.TotalAmount));
            cmd.Parameters.Add(new SqlParameter("@paidtoday", o.PaidToday));
            cmd.Parameters.Add(new SqlParameter("@isworkorder", o.IsWorkOrder));
            cmd.Parameters.Add(new SqlParameter("@orderdate", o.OrderDate));
            cmd.Parameters.Add(new SqlParameter("@shippingdate", o.ShippingDate));
            cmd.Parameters.Add(new SqlParameter("@comments", o.Comments));
            cmd.Parameters.Add(new SqlParameter("@orderid", o.OrderId));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteOrderById(int orderId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("delete from [order] where [orderid] = @orderid", con);
            cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void InsertOrderHistory(OrderHistory h)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("insert into [orderhistory] ([orderid], [historydate], [comments]) values (@orderid, @historydate, @comments)", con);
            cmd.Parameters.Add(new SqlParameter("@orderid", h.OrderId));
            cmd.Parameters.Add(new SqlParameter("@historydate", h.HistoryDate));
            cmd.Parameters.Add(new SqlParameter("@comments", h.Comments));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void UpdateOrderHistoryComments(OrderHistory h)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [orderhistory] set [comments] = @comments where [orderhistoryid] = @orderhistoryid", con);
            cmd.Parameters.Add(new SqlParameter("@comments", h.Comments));
            cmd.Parameters.Add(new SqlParameter("@orderhistoryid", h.OrderHistoryId));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteOrderHistoryById(int orderHistoryId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("delete from [orderhistory] where [orderhistoryid] = @orderhistoryid", con);
            cmd.Parameters.Add(new SqlParameter("@orderhistoryid", orderHistoryId));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<OrderHistory> GetOrderHistoryByOrderId(int orderId)
        {
            List<OrderHistory> histories = new List<OrderHistory>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select * from [orderhistory] where [orderid] = @orderid order by [historydate] desc", con);
            cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                OrderHistory h = new OrderHistory();
                h.OrderHistoryId = Convert.ToInt32(rd["orderhistoryid"]);
                h.OrderId = Convert.ToInt32(rd["orderid"]);
                h.HistoryDate = Convert.ToDateTime(rd["historydate"]);
                h.Comments = rd["comments"].ToString();
                histories.Add(h);
            }
            rd.Close();

            return histories;
        }

        public static List<History> GetHistoricSales(DateTime dt1, DateTime dt2, Category c)
        {
            List<History> histories = new List<History>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select [item].[itemname], sum(0 - [history].[quantity]) as [quantity] from [history] inner join [item] on [item].[itemid] = [history].[itemid] where [history].[action] <> N'Adjust' and [history].[actiondate] >= @dt1 and [history].[actiondate] <= @dt2", con);
            if (c.CategoryId > 0)
            {
                cmd.CommandText += " and [item].[categoryid] = " + c.CategoryId;
            }
            cmd.CommandText += " group by [item].[itemname] order by [quantity] desc";
            cmd.Parameters.Add(new SqlParameter("@dt1", dt1));
            cmd.Parameters.Add(new SqlParameter("@dt2", dt2));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rd.Read())
            {
                History h = new History();
                h.ItemName = rd["itemname"].ToString();
                h.Quantity = Convert.ToDouble(rd["quantity"]);
                histories.Add(h);
            }
            rd.Close();

            return histories;
        }

        public static DataTable RunQuery(string query)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            con.Open();
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
    }
}
