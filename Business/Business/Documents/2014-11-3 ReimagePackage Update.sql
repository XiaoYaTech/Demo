USE [MCD_AM]
GO

--2014-11-3  Adina.Zhang
ALTER table [dbo].[ReimagePackage] alter column CreateTime datetime NULL;
ALTER table [dbo].[ReimagePackage] alter column CreateUserAccount nvarchar(50) NULL;

--2014-11-4  Cary.Chen
ALTER table [dbo].[DataSync_LDW_AM_STFinanceData] 
	ADD SOI_Store_PreviousY1 nvarchar(25) NULL, 
		SOI_Store_PreviousY2 nvarchar(25) NULL,
		SOI_Store_PreviousY3 nvarchar(25) NULL;


--2014-11-5 Clay.Wang  Create Table GBMemo
/****** Object:  Table [dbo].[GBMemo]    Script Date: 2014-11-5 15:33:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GBMemo](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [nvarchar](50) NOT NULL,
	[ProcInstID] [int] NULL,
	[IsHistory] [bit] NOT NULL,
	[IsClosed] [bit] NOT NULL,
	[IsInOperation] [bit] NOT NULL,
	[IsMcCafe] [bit] NOT NULL,
	[IsKiosk] [bit] NOT NULL,
	[IsMDS] [bit] NOT NULL,
	[Is24Hour] [bit] NOT NULL,
	[ContractComments] [nvarchar](max) NULL,
	[GBMemoComments] [nvarchar](max) NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUserAccount] [nvarchar](50) NULL,
	[LastUpdateUserNameZHCN] [nvarchar](50) NULL,
	[LastUpdateUserNameENUS] [nvarchar](50) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserNameZHCN] [nvarchar](50) NULL,
	[CreateUserNameENUS] [nvarchar](50) NULL,
 CONSTRAINT [PK__GBMemo__3214EC071F398B65] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__IsHistor__2121D3D7]  DEFAULT ((0)) FOR [IsHistory]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__IsClosed__2215F810]  DEFAULT ((0)) FOR [IsClosed]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__IsInOper__230A1C49]  DEFAULT ((0)) FOR [IsInOperation]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__IsMcCafe__23FE4082]  DEFAULT ((0)) FOR [IsMcCafe]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__IsKiosk__24F264BB]  DEFAULT ((0)) FOR [IsKiosk]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__IsMDS__25E688F4]  DEFAULT ((0)) FOR [IsMDS]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__Is24Hour__26DAAD2D]  DEFAULT ((0)) FOR [Is24Hour]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__LastUpda__27CED166]  DEFAULT (getdate()) FOR [LastUpdateTime]
GO

ALTER TABLE [dbo].[GBMemo] ADD  CONSTRAINT [DF__GBMemo__CreateTi__28C2F59F]  DEFAULT (getdate()) FOR [CreateTime]
GO