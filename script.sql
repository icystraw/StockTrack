/****** Object:  Table [category]    Script Date: 5/11/2013 8:12:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [category](
	[categoryid] [int] IDENTITY(1,1) NOT NULL,
	[categoryname] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED 
(
	[categoryid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [history]    Script Date: 5/11/2013 8:12:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [history](
	[historyid] [int] IDENTITY(1,1) NOT NULL,
	[itemid] [int] NOT NULL,
	[entrydate] [datetime] NOT NULL,
	[actiondate] [datetime] NOT NULL,
	[action] [nvarchar](15) NOT NULL,
	[quantity] [float] NOT NULL,
	[orderno] [nvarchar](30) NOT NULL,
	[comments] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_history] PRIMARY KEY CLUSTERED 
(
	[historyid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [item]    Script Date: 5/11/2013 8:12:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [item](
	[itemid] [int] IDENTITY(1,1) NOT NULL,
	[categoryid] [int] NOT NULL,
	[itemname] [nvarchar](50) NOT NULL,
	[quantity] [float] NOT NULL,
 CONSTRAINT [PK_item] PRIMARY KEY CLUSTERED 
(
	[itemid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_history]    Script Date: 5/11/2013 8:12:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_history] ON [history]
(
	[orderno] ASC
) ON [PRIMARY]
GO
/****** Object:  Index [IX_history_1]    Script Date: 5/11/2013 8:12:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_history_1] ON [history]
(
	[entrydate] ASC
) ON [PRIMARY]
GO
/****** Object:  Index [IX_history_2]    Script Date: 5/11/2013 8:12:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_history_2] ON [history]
(
	[actiondate] ASC
) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_history_3]    Script Date: 5/11/2013 8:12:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_history_3] ON [history]
(
	[action] ASC
) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_item]    Script Date: 5/11/2013 8:12:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_item] ON [item]
(
	[itemname] ASC
) ON [PRIMARY]
GO
ALTER TABLE [history]  WITH CHECK ADD  CONSTRAINT [FK_history_item] FOREIGN KEY([itemid])
REFERENCES [item] ([itemid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [history] CHECK CONSTRAINT [FK_history_item]
GO
ALTER TABLE [item]  WITH CHECK ADD  CONSTRAINT [FK_item_category] FOREIGN KEY([categoryid])
REFERENCES [category] ([categoryid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [item] CHECK CONSTRAINT [FK_item_category]
GO
