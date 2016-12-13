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

        public static List<Item> GetRelatedItems(int itemId)
        {
            List<Item> items = new List<Item>();

            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select top 25 i.[itemid], i.[itemname], i.[quantity], i.[categoryid], count(i.[itemid]) as [frequence] from [item] i, [history] h where h.[itemid] = i.[itemid] and h.[action] = N'Purchase' and h.[orderid] in (select distinct o.[orderid] from [order] o inner join [history] h on o.[orderid] = h.[orderid] where h.[itemid] = @itemid and h.[action] = N'Purchase') group by i.[itemid], i.[itemname], i.[quantity], i.[categoryid] order by [frequence] desc", con);
            cmd.Parameters.Add(new SqlParameter("@itemid", itemId));
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

        public static DataTable GetRelatedItemsForItemInsight(int itemId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlDataAdapter da = new SqlDataAdapter("declare @totalsales float = 0; select @totalsales = count(*) from [history] where [itemid] = @itemid and [action] = N'Purchase'; select top 50 i.[itemid], i.[itemname], count(i.[itemid]) / @totalsales as [frequence] from [item] i, [history] h where h.[itemid] = i.[itemid] and h.[action] = N'Purchase' and h.[orderid] in (select distinct o.[orderid] from [order] o inner join [history] h on o.[orderid] = h.[orderid] where h.[itemid] = @itemid and h.[action] = N'Purchase') group by i.[itemid], i.[itemname] order by [frequence] desc", con);
            da.SelectCommand.Parameters.Add(new SqlParameter("@itemid", itemId));
            con.Open();
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            if (dt.Rows.Count > 0) dt.Rows.RemoveAt(0);
            return dt;
        }

        public static DataTable GetItemMonthlySales(int itemId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlDataAdapter da = new SqlDataAdapter("select month([actiondate]) as [month], year([actiondate]) as [year], sum(0 - [quantity]) as [quantity] from [history] where [action] <> N'Adjust' and [itemid] = @itemid group by month([actiondate]), year([actiondate]) order by [year] desc, [month] desc", con);
            da.SelectCommand.Parameters.Add(new SqlParameter("@itemid", itemId));
            con.Open();
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public static int AddItem(Item i)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("insert into [item] ([categoryid], [itemname], [quantity]) values (@categoryid, @itemname, 0); select @@identity;", con);
            cmd.Parameters.Add(new SqlParameter("@categoryid", i.CategoryId));
            cmd.Parameters.Add(new SqlParameter("@itemname", i.ItemName));
            con.Open();
            int retVal = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return retVal;
        }

        public static double QtyInWork(int itemId, byte isWorkOrder)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select 0 - sum([quantity]) as [quantity] from [history], [order] where [order].[orderid] = [history].[orderid] and [history].[action] <> N'Adjust' and [order].[isworkorder] = " + isWorkOrder + " and [history].[itemid] = @itemid", con);
            cmd.Parameters.Add(new SqlParameter("@itemid", itemId));
            con.Open();
            double retVal = 0;
            try
            {
                retVal = Convert.ToDouble(cmd.ExecuteScalar());
            }
            catch { }
            con.Close();
            return retVal;
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
            cmd.Parameters.Add(new SqlParameter("@orderid", h.OrderId));
            cmd.Parameters.Add(new SqlParameter("@comments", h.Comments));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void UpdateHistory(History h)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [history] set [comments] = @comments, [action] = @action, [quantity] = @quantity where [historyid] = @historyid", con);
            cmd.Parameters.Add(new SqlParameter("@comments", h.Comments));
            cmd.Parameters.Add(new SqlParameter("@action", h.Action));
            cmd.Parameters.Add(new SqlParameter("@quantity", h.Quantity));
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
            SqlCommand cmd = new SqlCommand("select [history].*, [order].[orderno], [order].[isworkorder], [order].[customername], [order].[contactno] from [history] inner join [order] on [order].[orderid] = [history].[orderid] where [itemid] = @itemid order by [entrydate] desc", con);
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
                h.IsWorkOrder = Convert.ToByte(rd["isworkorder"]);
                h.CustomerName = rd["customername"].ToString();
                h.ContactNo = rd["contactno"].ToString();
                histories.Add(h);
            }
            rd.Close();

            return histories;
        }

        public static List<History> GetHistoryByOrderId(int orderId)
        {
            List<History> histories = new List<History>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select [history].*, [item].[itemname], [order].[orderno], [order].[isworkorder] from [history] inner join [item] on [item].[itemid] = [history].[itemid] inner join [order] on [order].[orderid] = [history].[orderid] where [history].[orderid] = @orderid order by [entrydate] desc", con);
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
                h.IsWorkOrder = Convert.ToByte(rd["isworkorder"]);
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
                o.IsWorkOrder = Convert.ToByte(rd["isworkorder"]);
                o.OrderDate = Convert.ToDateTime(rd["orderdate"]);
                o.ShippingDate = Convert.ToDateTime(rd["shippingdate"]);
                o.Comments = rd["comments"].ToString();
                o.Folder = rd["folder"].ToString();
                o.Email = rd["email"].ToString();
                o.Address = rd["address"].ToString();
            }
            rd.Close();

            return o;
        }

        public static List<Order> GetOrderByNo(string orderNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("select * from [order] where [orderno] = @orderno", con);
            cmd.Parameters.Add(new SqlParameter("@orderno", orderNo));
            con.Open();
            IDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            List<Order> os = new List<Order>();
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
                o.IsWorkOrder = Convert.ToByte(rd["isworkorder"]);
                o.OrderDate = Convert.ToDateTime(rd["orderdate"]);
                o.ShippingDate = Convert.ToDateTime(rd["shippingdate"]);
                o.Comments = rd["comments"].ToString();
                o.Folder = rd["folder"].ToString();
                o.Email = rd["email"].ToString();
                o.Address = rd["address"].ToString();
                os.Add(o);
            }
            rd.Close();

            return os;
        }

        public static List<Order> SearchOrder(string keyword, DateTime? startDate, DateTime? endDate, DateTime? startSDate, DateTime? endSDate, byte? isWorkOrder)
        {
            List<Order> orders = new List<Order>();
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "select * from (select [order].*, (select top 1 [comments] from [orderhistory] where [orderid] = [order].[orderid]";
            if (!string.IsNullOrEmpty(keyword))
            {
                cmd.CommandText += " and [comments] like N'%' + @keyword + N'%'";
            }
            cmd.CommandText += " order by [historydate] desc) as [progress],";
            if (!string.IsNullOrEmpty(keyword))
            {
                cmd.CommandText += " (select top 1 [i].[itemname] from [item] as [i], [history] as [h] where [h].[itemid] = [i].[itemid] and [h].[orderid] = [order].[orderid] and [i].[itemname] like N'%' + @keyword + N'%') as [itemname]";
            }
            else
            {
                cmd.CommandText += " N'' as [itemname]";
            }
            cmd.CommandText += " from [order] where 1 = 1";
            if (startDate != null)
            {
                cmd.CommandText += " and [order].[orderdate] >= @startdate";
                cmd.Parameters.Add(new SqlParameter("@startdate", startDate));
            }
            if (endDate != null)
            {
                cmd.CommandText += " and [order].[orderdate] < @enddate";
                cmd.Parameters.Add(new SqlParameter("@enddate", ((DateTime)endDate).AddDays(1)));
            }
            if (startSDate != null)
            {
                cmd.CommandText += " and [order].[shippingdate] >= @startsdate";
                cmd.Parameters.Add(new SqlParameter("@startsdate", startSDate));
            }
            if (endSDate != null)
            {
                cmd.CommandText += " and [order].[shippingdate] < @endsdate";
                cmd.Parameters.Add(new SqlParameter("@endsdate", ((DateTime)endSDate).AddDays(1)));
            }
            if (isWorkOrder != null)
            {
                cmd.CommandText += " and [order].[isworkorder] = @isworkorder";
                cmd.Parameters.Add(new SqlParameter("@isworkorder", isWorkOrder));
            }
            cmd.CommandText += ") as [temptable]";
            if (!string.IsNullOrEmpty(keyword))
            {
                cmd.CommandText += " where [itemname] like N'%' + @keyword + N'%' or [shipping] like N'%' + @keyword + N'%' or [orderno] like N'%' + @keyword + N'%' or [customername] like N'%' + @keyword + N'%' or [contactno] like N'%' + @keyword + N'%' or [comments] like N'%' + @keyword + N'%' or [progress] like N'%' + @keyword + N'%' or [email] like N'%' + @keyword + N'%' or [address] like N'%' + @keyword + N'%'";
                cmd.Parameters.Add(new SqlParameter("@keyword", keyword));
            }
            cmd.CommandText += " OPTION (RECOMPILE)";
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
                o.IsWorkOrder = Convert.ToByte(rd["isworkorder"]);
                o.OrderDate = Convert.ToDateTime(rd["orderdate"]);
                o.ShippingDate = Convert.ToDateTime(rd["shippingdate"]);
                o.Comments = rd["comments"].ToString();
                o.Folder = rd["folder"].ToString();
                o.Email = rd["email"].ToString();
                o.Address = rd["address"].ToString();
                o.Progress = rd["progress"].ToString();
                if (string.IsNullOrEmpty(o.Progress) && !string.IsNullOrEmpty(keyword))
                    o.Progress = "No match found in history";
                orders.Add(o);
            }
            rd.Close();

            return orders;
        }

        public static void UpdateOrder(Order o)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("update [order] set [orderno] = @orderno, [customername] = @customername, [contactno] = @contactno, [shipping] = @shipping, [totalamount] = @totalamount, [paidtoday] = @paidtoday, [isworkorder] = @isworkorder, [orderdate] = @orderdate, [shippingdate] = @shippingdate, [comments] = @comments, [folder] = @folder, [email] = @email, [address] = @address where [orderid] = @orderid", con);
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
            cmd.Parameters.Add(new SqlParameter("@folder", o.Folder));
            cmd.Parameters.Add(new SqlParameter("@email", o.Email));
            cmd.Parameters.Add(new SqlParameter("@address", o.Address));
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

        public static int AddNewWorkOrder(string orderNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("insert into [order] ([orderno], [customername], [contactno], [shipping], [totalamount], [paidtoday], [isworkorder], [orderdate], [shippingdate], [comments], [folder], [email], [address]) values (@orderno, N'', N'', N'', 0, 0, 1, GETDATE(), GETDATE(), N'', N'', N'', N''); select @@identity;", con);
            cmd.Parameters.Add(new SqlParameter("@orderno", orderNo));
            con.Open();
            int retVal = 0;
            retVal = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return retVal;
        }

        public static int AddNewTentativeOrder(string orderNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("insert into [order] ([orderno], [customername], [contactno], [shipping], [totalamount], [paidtoday], [isworkorder], [orderdate], [shippingdate], [comments], [folder], [email], [address]) values (@orderno, N'', N'', N'', 0, 0, 2, GETDATE(), GETDATE(), N'', N'', N'', N''); select @@identity;", con);
            cmd.Parameters.Add(new SqlParameter("@orderno", orderNo));
            con.Open();
            int retVal = 0;
            retVal = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return retVal;
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
            SqlCommand cmd = new SqlCommand("select [item].[itemname], sum(0 - [history].[quantity]) as [quantity] from [history] inner join [item] on [item].[itemid] = [history].[itemid] inner join [order] on [order].[orderid] = [history].[orderid] where [history].[action] <> N'Adjust' and [history].[actiondate] >= @dt1 and [history].[actiondate] <= @dt2 and [order].[isworkorder] <> 2", con);
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

        public static void MergeItems(int fromId, int toId)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "declare @totalqty int = 0;";
            cmd.CommandText += "select @totalqty = sum([quantity]) from [item] where [itemid] = " + fromId + " or [itemid] = " + toId + ";";
            cmd.CommandText += "update [item] set [quantity] = @totalqty where [itemid] = " + toId + ";";
            cmd.CommandText += "update [history] set [itemid] = " + toId + " where [itemid] = " + fromId + ";";
            cmd.CommandText += "delete from [item] where [itemid] = " + fromId + ";";
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
