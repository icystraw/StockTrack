SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [inserthistory]
	@itemid int,
	@entrydate datetime,
	@actiondate datetime,
	@action nvarchar(15),
	@quantity float,
	@orderno nvarchar(30),
	@orderid int,
	@comments nvarchar(150)
as
begin
	declare @oid int;
	if @orderid = 0
	begin 
		select top 1 @oid = [orderid] from [order] where [orderno] = @orderno order by [orderdate] desc;
		if @oid is null
		begin
			insert into [order] ([orderno], [customername], [contactno], [shipping], [totalamount],
			[paidtoday], [isworkorder], [orderdate], [shippingdate], [comments], [folder], [email], [address]) values
			(@orderno, N'', N'', N'', 0, 0, 0, GETDATE(), GETDATE(), N'', N'', N'', N'');
			select @oid = @@identity;
		end;
	end
	else
	begin
		set @oid = @orderid;
	end;

	insert into [history]
	([itemid], [entrydate], [actiondate], [action], [quantity], [orderid], [comments])
	values
	(@itemid, @entrydate, @actiondate, @action, @quantity, @oid, @comments);
end;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [category](
	[categoryid] [int] IDENTITY(1,1) NOT NULL,
	[categoryname] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED 
(
	[categoryid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [history](
	[historyid] [int] IDENTITY(1,1) NOT NULL,
	[itemid] [int] NOT NULL,
	[entrydate] [datetime] NOT NULL,
	[actiondate] [datetime] NOT NULL,
	[action] [nvarchar](50) NOT NULL,
	[quantity] [float] NOT NULL,
	[orderid] [int] NOT NULL,
	[comments] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_history] PRIMARY KEY CLUSTERED 
(
	[historyid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [item](
	[itemid] [int] IDENTITY(1,1) NOT NULL,
	[categoryid] [int] NOT NULL,
	[itemname] [nvarchar](100) NOT NULL,
	[quantity] [float] NOT NULL,
 CONSTRAINT [PK_item] PRIMARY KEY CLUSTERED 
(
	[itemid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [order](
	[orderid] [int] IDENTITY(1,1) NOT NULL,
	[orderno] [nvarchar](50) NOT NULL,
	[customername] [nvarchar](100) NOT NULL,
	[contactno] [nvarchar](50) NOT NULL,
	[shipping] [nvarchar](50) NOT NULL,
	[totalamount] [float] NOT NULL,
	[paidtoday] [float] NOT NULL,
	[isworkorder] [tinyint] NOT NULL,
	[orderdate] [datetime] NOT NULL,
	[shippingdate] [datetime] NOT NULL,
	[comments] [nvarchar](200) NOT NULL,
	[folder] [nvarchar](300) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[address] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_order] PRIMARY KEY CLUSTERED 
(
	[orderid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [orderhistory](
	[orderhistoryid] [int] IDENTITY(1,1) NOT NULL,
	[orderid] [int] NOT NULL,
	[historydate] [datetime] NOT NULL,
	[comments] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_orderhistory] PRIMARY KEY CLUSTERED 
(
	[orderhistoryid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_history_1] ON [history]
(
	[entrydate] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_history_2] ON [history]
(
	[actiondate] ASC
) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_history_3] ON [history]
(
	[action] ASC
) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_item] ON [item]
(
	[itemname] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order] ON [order]
(
	[isworkorder] ASC
) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_order_1] ON [order]
(
	[orderno] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order_2] ON [order]
(
	[customername] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order_3] ON [order]
(
	[contactno] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order_4] ON [order]
(
	[shipping] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order_5] ON [order]
(
	[comments] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order_6] ON [order]
(
	[email] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_order_7] ON [order]
(
	[address] ASC
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_orderhistory_1] ON [orderhistory]
(
	[comments] ASC
) ON [PRIMARY]
GO
ALTER TABLE [history]  WITH CHECK ADD  CONSTRAINT [FK_history_item] FOREIGN KEY([itemid])
REFERENCES [item] ([itemid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [history] CHECK CONSTRAINT [FK_history_item]
GO
ALTER TABLE [history]  WITH CHECK ADD  CONSTRAINT [FK_history_order] FOREIGN KEY([orderid])
REFERENCES [order] ([orderid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [history] CHECK CONSTRAINT [FK_history_order]
GO
ALTER TABLE [item]  WITH CHECK ADD  CONSTRAINT [FK_item_category] FOREIGN KEY([categoryid])
REFERENCES [category] ([categoryid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [item] CHECK CONSTRAINT [FK_item_category]
GO
ALTER TABLE [orderhistory]  WITH CHECK ADD  CONSTRAINT [FK_orderhistory_order] FOREIGN KEY([orderid])
REFERENCES [order] ([orderid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [orderhistory] CHECK CONSTRAINT [FK_orderhistory_order]
GO
