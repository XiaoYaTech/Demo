USE [MCD_AM]
GO

/****** Object:  Table [dbo].[Attachment]    Script Date: 2014/6/30 21:14:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Attachment](
	[ID] [uniqueidentifier] NOT NULL,
	[RefTableID] [nvarchar](50) NULL,
	[RefTableName] [nvarchar](100) NULL,
	[Name] [nvarchar](256) NULL,
	[Extension] [nvarchar](50) NULL,
	[RelativePath] [nvarchar](256) NULL,
	[InternalName] [nvarchar](256) NULL,
	[ContentType] [nvarchar](100) NULL,
	[Length] [int] NULL,
	[TypeCode] [nvarchar](50) NULL,
	[CreatorID] [nvarchar](50) NULL,
	[CreatorName] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[Sequence] [int] NULL,
	[IsDelete] [int] NULL,
 CONSTRAINT [PK_Core_Attachment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Attachment] ADD  CONSTRAINT [DF_Attachment_ID]  DEFAULT (newid()) FOR [ID]
GO

ALTER TABLE [dbo].[Attachment] ADD  CONSTRAINT [DF_Attachment_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO

ALTER TABLE [dbo].[Attachment] ADD  CONSTRAINT [DF_Attachment_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO


/****** Object:  Table [dbo].[ClosureCommens]    Script Date: 2014/6/30 21:15:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureCommens](
	[Id] [uniqueidentifier] NOT NULL,
	[UserAccount] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[RefTableId] [uniqueidentifier] NULL,
	[RefTableName] [nvarchar](50) NULL,
	[TitleCode] [nvarchar](50) NULL,
	[TitleName] [nvarchar](50) NULL,
	[Action] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[Content] [nvarchar](500) NULL,
	[TypeCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_ClosureCommens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/****** Object:  Table [dbo].[ClosureContractInfo]    Script Date: 2014/6/30 21:15:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureContractInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[FirstParty] [nvarchar](120) NULL,
	[USCodo] [nvarchar](120) NULL,
	[MCDOwersShop] [nvarchar](50) NULL,
	[OwnerContact] [nvarchar](50) NULL,
	[ContractModeName] [nvarchar](50) NULL,
	[RentType] [nvarchar](50) NULL,
	[Area] [float] NULL,
	[Type] [int] NULL,
	[EstimateStartTime] [datetime] NULL,
	[EstimateEndTime] [datetime] NULL,
	[ContractStartTime] [datetime] NULL,
	[ContractEndTime] [datetime] NULL,
	[LastSubmitTime] [datetime] NULL,
	[Is2010YearModefied] [bit] NULL,
	[RentStructure] [nvarchar](50) NULL,
	[IsEarlierDate] [bit] NULL,
	[EarlierDateDesc] [nvarchar](500) NULL,
	[RentPayer] [nvarchar](50) NULL,
	[RentPayWay] [nvarchar](50) NULL,
	[IsDepositeMoney] [bit] NULL,
	[DepositeMoney] [money] NULL,
	[IsDepositeRefund] [bit] NULL,
	[DepositeRefundDate] [datetime] NULL,
	[IsOverduePayClause] [nchar](10) NULL,
	[IsBankGuarantee] [bit] NULL,
	[BankGuaranteeCode] [nchar](10) NULL,
	[BankGuaranteeMoney] [money] NULL,
	[BankGuaranteeStartTime] [datetime] NULL,
	[BankGuaranteeEndTime] [datetime] NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserName] [nvarchar](50) NULL,
	[PurchaseAuthorityYear] [int] NULL,
	[MaturityYear] [int] NULL,
 CONSTRAINT [PK_ClosureContractInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureContractInfo] ADD  CONSTRAINT [DF_ClosureContractInfo_Id]  DEFAULT (newid()) FOR [Id]
GO


/****** Object:  Table [dbo].[ClosureContractRefund]    Script Date: 2014/6/30 21:15:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureContractRefund](
	[Id] [uniqueidentifier] NOT NULL,
	[ContractId] [uniqueidentifier] NULL,
	[RefundTime] [datetime] NULL,
	[IsRental] [bit] NULL,
	[IsRentalDesc] [nvarchar](120) NULL,
	[IsRedLine] [bit] NULL,
	[IsRedLineDesc] [nvarchar](120) NULL,
	[IsLeaseTerm] [bit] NULL,
	[IsLeaseTermDesc] [nvarchar](120) NULL,
	[CreateTime] [datetime] NULL,
	[CreaetUserAccount] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_ClosureContractRefund] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureContractRefund] ADD  CONSTRAINT [DF_ClosureContractRefund_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO


/****** Object:  Table [dbo].[ClosureExecutiveSummary]    Script Date: 2014/6/30 21:15:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureExecutiveSummary](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[CreatorAccount] [nvarchar](50) NULL,
	[CreatorName] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
 CONSTRAINT [PK_ClosureExecutiveSummary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureExecutiveSummary] ADD  CONSTRAINT [DF_ClosureExecutiveSummary_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ClosureExecutiveSummary] ADD  CONSTRAINT [DF_ClosureExecutiveSummary_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO


/****** Object:  Table [dbo].[ClosureInfo]    Script Date: 2014/6/30 21:15:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NOT NULL,
	[USCode] [nvarchar](50) NULL,
	[EstimatedCloseDate] [datetime] NULL,
	[ActualCloseDate] [datetime] NULL,
	[ClosureTypeCode] [nvarchar](50) NULL,
	[ClosureTypeName] [nvarchar](50) NULL,
	[RiskStatusCode] [nvarchar](50) NOT NULL,
	[RiskStatusName] [nvarchar](50) NULL,
	[ClosureReasonCode] [nvarchar](50) NULL,
	[ClosureReasonName] [nvarchar](50) NULL,
	[RelocationCode] [nvarchar](50) NULL,
	[RelocationName] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserName] [nvarchar](50) NULL,
	[RepUserAccount] [nvarchar](50) NULL,
	[AssetRepAccount] [nvarchar](50) NULL,
	[AssetRepName] [nvarchar](50) NULL,
	[AssetActorAccount] [nvarchar](50) NULL,
	[AssetActorName] [nvarchar](50) NULL,
	[FinanceAccount] [nvarchar](50) NULL,
	[FinanceName] [nvarchar](50) NULL,
	[PMAccount] [nvarchar](50) NULL,
	[PMName] [nvarchar](50) NULL,
	[LegalAccount] [nvarchar](50) NULL,
	[LegalName] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_ClosureInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureInfo] ADD  CONSTRAINT [DF_ClosureInfo_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ClosureInfo] ADD  CONSTRAINT [DF_ClosureInfo_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'USCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计关店的时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'EstimatedCloseDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实际关店的时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'ActualCloseDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关店的类型的编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'ClosureTypeCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关店类型的名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'ClosureTypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险状态的编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'RiskStatusCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险状态的名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'RiskStatusName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关店的理由的编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'ClosureReasonCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关店理由的名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'ClosureReasonName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'安置的编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'RelocationCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'安置的名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'RelocationName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人的账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'CreateUserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人的姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureInfo', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO


/****** Object:  Table [dbo].[ClosureLegalReview]    Script Date: 2014/6/30 21:16:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureLegalReview](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[CreateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserName] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
	[IsAvailable] [bit] NULL,
	[LegalCommers] [nvarchar](500) NULL,
 CONSTRAINT [PK_ClosureLegalReview] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureLegalReview] ADD  CONSTRAINT [DF_ClosureLegalReview_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ClosureLegalReview] ADD  CONSTRAINT [DF_ClosureLegalReview_IsAvailable]  DEFAULT ((1)) FOR [IsAvailable]
GO


/****** Object:  Table [dbo].[ClosurePackage]    Script Date: 2014/6/30 21:16:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosurePackage](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[OriginalCFNPV] [float] NULL,
	[NewSiteNetCFNPV] [float] NULL,
	[OtherCFNPV] [float] NULL,
	[NetGain] [float] NULL,
	[NetOperatingIncome] [decimal](19, 4) NULL,
	[IsRelocation] [bit] NULL,
	[RelocationPipelineID] [int] NULL,
	[PipelineName] [nvarchar](50) NULL,
	[ReasonDescriptionForNegativeNetGain] [nvarchar](500) NULL,
	[CreateTime] [datetime] NULL,
	[LastUpdateTime] [datetime] NULL,
	[LastUpdateUserAccount] [nvarchar](50) NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_ClosurePackage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosurePackage] ADD  CONSTRAINT [DF_ClosurePackage_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO


/****** Object:  Table [dbo].[ClosureProjectTeam]    Script Date: 2014/6/30 21:16:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureProjectTeam](
	[Id] [uniqueidentifier] NOT NULL,
	[memberTypeCode] [nvarchar](50) NULL,
	[memberTypeName] [nvarchar](50) NULL,
	[UserAccount] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserName] [nvarchar](50) NULL,
	[LastUpdateUserAccount] [nvarchar](50) NULL,
	[LastUpdateDate] [datetime] NULL,
	[Sequence] [int] NULL,
	[ProjectId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ClosureProjectTeam] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureProjectTeam] ADD  CONSTRAINT [DF_ClosureProjectTeam_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'成员类型编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'memberTypeCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'成员类型名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'memberTypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'UserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'UserName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'CreateUserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'LastUpdateUserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'LastUpdateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureProjectTeam', @level2type=N'COLUMN',@level2name=N'Sequence'
GO


/****** Object:  Table [dbo].[ClosureTool]    Script Date: 2014/6/30 21:16:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureTool](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[TotalSales_Adjustment_RMB] [decimal](19, 4) NULL,
	[CompSales_Adjustment] [int] NULL,
	[CompSalesMacket_Adjustment] [int] NULL,
	[CompCG_Adjustment] [int] NULL,
	[CompCGMacket_Adjustment] [int] NULL,
	[PAC_RMB_Adjustment] [decimal](19, 4) NULL,
	[PAC_Adjustment] [int] NULL,
	[PACMarket_Adjustment] [int] NULL,
	[Rent_RMB_Adjustment] [decimal](19, 4) NULL,
	[DepreciationLHI_RMB_Adjustment] [decimal](19, 4) NULL,
	[ServiceFee_RMB_Adjustment] [decimal](19, 4) NULL,
	[Accounting_RMB_Adjustment] [decimal](19, 4) NULL,
	[Insurance_RMB_Adjustment] [decimal](19, 4) NULL,
	[TaxesLicenses_RMB_Adjustment] [decimal](19, 4) NULL,
	[Depreciation_ESSD_RMB_Adjustment] [decimal](19, 4) NULL,
	[Interest_ESSD_RMB_Adjustment] [decimal](19, 4) NULL,
	[OtherIncExp_RMB_Adjustment] [decimal](19, 4) NULL,
	[NonProduct_Sales_RMB_Adjustment] [decimal](19, 4) NULL,
	[NonProduct_Costs_RMB_Adjustment] [decimal](19, 4) NULL,
	[TotalSales_RMB] [decimal](19, 4) NULL,
	[CompSales] [int] NULL,
	[CompSalesMacket] [int] NULL,
	[CompCG] [int] NULL,
	[CompCGMacket] [int] NULL,
	[PAC_RMB] [decimal](19, 4) NULL,
	[PAC] [int] NULL,
	[PACMarket] [int] NULL,
	[Rent_RMB] [decimal](19, 4) NULL,
	[DepreciationLHI_RMB] [decimal](19, 4) NULL,
	[ServiceFee_RMB] [decimal](19, 4) NULL,
	[Accounting_RMB] [decimal](19, 4) NULL,
	[Insurance_RMB] [decimal](19, 4) NULL,
	[TaxesLicenses_RMB] [decimal](19, 4) NULL,
	[Depreciation_ESSD_RMB] [decimal](19, 4) NULL,
	[Interest_ESSD_RMB] [decimal](19, 4) NULL,
	[OtherIncExp_RMB] [decimal](19, 4) NULL,
	[NonProduct_Sales_RMB] [decimal](19, 4) NULL,
	[NonProduct_Costs_RMB] [decimal](19, 4) NULL,
	[SOI] [int] NULL,
	[SOIMarket] [int] NULL,
	[CashFlow_RMB] [int] NULL,
	[CreateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CompSales_TTMY1] [decimal](19, 4) NULL,
	[CompSales_TTMY2] [decimal](19, 4) NULL,
	[CompSales_Market_TTMY1] [int] NULL,
	[CompSales_Market_TTMY2] [int] NULL,
	[CompGC_TTMY1] [decimal](19, 4) NULL,
	[CompGC_TTMY2] [decimal](19, 4) NULL,
	[CompGCMarket_TTMY1] [decimal](19, 4) NULL,
	[CompGCMarket_TTMY2] [decimal](19, 4) NULL,
	[PAC_TTMY1] [decimal](19, 4) NULL,
	[PAC_TTMY2] [decimal](19, 4) NULL,
	[PACMarket_TTMY1] [int] NULL,
	[PACMarket_TTMY2] [int] NULL,
	[SOI_TTMY1] [int] NULL,
	[SOI_TTMY2] [int] NULL,
	[SOIMarket_TTMY1] [int] NULL,
	[SOIMarket_TTMY2] [int] NULL,
	[CashFlow_TTMY1] [decimal](19, 4) NULL,
	[CashFlow_TTMY2] [decimal](19, 4) NULL,
	[Status] [int] NULL,
	[TotalSales_TTMY1] [decimal](19, 4) NULL,
	[TotalSales_TTMY2] [decimal](19, 4) NULL,
 CONSTRAINT [PK_ClosureTool] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/****** Object:  Table [dbo].[ClosureUsers]    Script Date: 2014/6/30 21:16:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[UserAccount] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[RoleCode] [nvarchar](50) NULL,
	[RoleName] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserName] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_ClosureNoticeUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureUsers] ADD  CONSTRAINT [DF_ClosureNoticeUsers_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ClosureUsers] ADD  CONSTRAINT [DF_ClosureNoticeUsers_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [dbo].[ClosureUsers] ADD  CONSTRAINT [DF_ClosureNoticeUsers_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureUsers', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClosureUsers', @level2type=N'COLUMN',@level2name=N'UserAccount'
GO


/****** Object:  Table [dbo].[ClosureWOCheckList]    Script Date: 2014/6/30 21:16:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClosureWOCheckList](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NULL,
	[RE_Original] [money] NULL,
	[LHI_Original] [money] NULL,
	[ESSD_Original] [money] NULL,
	[Equipment_Original] [money] NULL,
	[Signage_Original] [money] NULL,
	[Seating_Original] [money] NULL,
	[Decoration_Original] [money] NULL,
	[TotalCost_Original] [money] NULL,
	[RE_NBV] [money] NULL,
	[LHI_NBV] [money] NULL,
	[ESSD_NBV] [money] NULL,
	[Equipment_NBV] [money] NULL,
	[Signage_NBV] [money] NULL,
	[Seating_NBV] [money] NULL,
	[Decoration_NBV] [money] NULL,
	[TotalCost_NBV] [money] NULL,
	[ClosingCost] [money] NULL,
	[EquipmentTransfer] [money] NULL,
	[TotalWriteOFF] [money] NULL,
	[RECost_WriteOFF] [money] NULL,
	[LHI_WriteOFF] [money] NULL,
	[ESSD_WriteOFF] [money] NULL,
	[Equipment_WriteOFF] [money] NULL,
	[Signage_WriteOFF] [money] NULL,
	[Seating_WriteOFF] [money] NULL,
	[Decoration_WriteOFF] [money] NULL,
	[TotalCost_WriteOFF] [money] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[CreateUserName] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[LastUpdateTime] [datetime] NULL,
	[LastUpdateUserAccount] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
	[Status] [int] NULL,
	[ProgressRate] [float] NULL,
	[IsAvailable] [bit] NULL,
	[Compensation] [nvarchar](50) NULL,
 CONSTRAINT [PK_ClosureWOCheckList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClosureWOCheckList] ADD  CONSTRAINT [DF_ClosureWOCheckList_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ClosureWOCheckList] ADD  CONSTRAINT [DF_ClosureWOCheckList_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO


/****** Object:  Table [dbo].[Dictionary]    Script Date: 2014/6/30 21:17:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Dictionary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameZHCN] [nvarchar](256) NOT NULL,
	[NameENUS] [nvarchar](256) NULL,
	[Value] [nvarchar](256) NOT NULL,
	[Code] [nvarchar](256) NOT NULL,
	[ParentCode] [nvarchar](256) NOT NULL,
	[IsDirectory] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[LastUpdateTime] [datetime] NULL,
	[Sequence] [int] NOT NULL,
	[ExtendField0] [nvarchar](256) NULL,
	[ExtendField1] [nvarchar](256) NULL,
	[ExtendField2] [nvarchar](256) NULL,
	[ExtendField3] [nvarchar](256) NULL,
	[ExtendField4] [nvarchar](256) NULL,
	[ExtendField5] [nvarchar](256) NULL,
 CONSTRAINT [PK__Dictiona__3214EC07CF8FA1E5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Dictionary] ADD  CONSTRAINT [DF_Dictionary_NameZHCN]  DEFAULT (N'中文名称') FOR [NameZHCN]
GO

ALTER TABLE [dbo].[Dictionary] ADD  CONSTRAINT [DF_Dictionary_NameENUS]  DEFAULT (N'英文名称') FOR [NameENUS]
GO

ALTER TABLE [dbo].[Dictionary] ADD  CONSTRAINT [DF_Dictionary_Code]  DEFAULT (N'编号') FOR [Code]
GO

ALTER TABLE [dbo].[Dictionary] ADD  CONSTRAINT [DF_Dictionary_ParentCode]  DEFAULT (N'父节点编号') FOR [ParentCode]
GO

ALTER TABLE [dbo].[Dictionary] ADD  CONSTRAINT [DF__Dictionar__Creat__108B795B]  DEFAULT (getdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Dictionary] ADD  CONSTRAINT [DF__Dictionar__Seque__117F9D94]  DEFAULT ((0)) FOR [Sequence]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中文名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'NameZHCN'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'英文名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'NameENUS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'Value'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'Code'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父节点编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ParentCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是目录' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'IsDirectory'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'CreateUserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'Sequence'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扩展字段0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ExtendField0'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扩展字段1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ExtendField1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扩展字段2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ExtendField2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扩展字段3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ExtendField3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扩展字段4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ExtendField4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扩展字段5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Dictionary', @level2type=N'COLUMN',@level2name=N'ExtendField5'
GO


/****** Object:  Table [dbo].[FABaseInfo]    Script Date: 2014/6/30 21:17:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FABaseInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreCode] [nvarchar](20) NULL,
	[Total_Net_Sales] [nvarchar](25) NULL,
	[CashFlow] [nvarchar](25) NULL,
	[SOI] [nvarchar](25) NULL,
	[RectPaidLL_YTD] [nvarchar](25) NULL,
	[McOpCoMargin] [nvarchar](25) NULL,
	[SOIPct] [nvarchar](25) NULL,
	[AdjRent_TTM] [nvarchar](25) NULL,
	[AdjRentPct_TTM] [nvarchar](25) NULL,
	[CompsSalesPct] [nvarchar](25) NULL,
	[LHI_NBV] [nvarchar](25) NULL,
	[ESSD_NBV] [nvarchar](25) NULL,
	[Total_NBV] [nvarchar](25) NULL,
	[CreatedTime] [datetime] NULL,
 CONSTRAINT [PK_FA_BASE_INFO] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅编号 US Code' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'StoreCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total Net Sales' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'Total_Net_Sales'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cash Flow(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'CashFlow'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'SOI'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'YTD Rect Paid to LL(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'RectPaidLL_YTD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'McOpCo Margin(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'McOpCoMargin'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI%' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'SOIPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TTM Adj Rent(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'AdjRent_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TTM Adj Rent%' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'AdjRentPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comps Sales %' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'CompsSalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'LHI NBV(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'LHI_NBV'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ESSD NBV(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'ESSD_NBV'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total NBV(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'Total_NBV'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FABaseInfo', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO


/****** Object:  Table [dbo].[FAData]    Script Date: 2014/6/30 21:17:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FAData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreCode] [nvarchar](20) NULL,
	[Total_Sales_TTM] [nvarchar](25) NULL,
	[Net_Product_Sales_TTM] [nvarchar](25) NULL,
	[GC_TTM] [nvarchar](25) NULL,
	[Total_Sales_Y1] [nvarchar](25) NULL,
	[CompSales_Y2] [nvarchar](25) NULL,
	[CompSales_Y3] [nvarchar](25) NULL,
	[CompSales_Y4] [nvarchar](25) NULL,
	[CompSales_TTM] [nvarchar](25) NULL,
	[CompGC_TTM] [nvarchar](25) NULL,
	[CompSales_CurrentYear] [nvarchar](25) NULL,
	[CompSales_Store_PreviousY1] [nvarchar](25) NULL,
	[CompSales_Store_PreviousY2] [nvarchar](25) NULL,
	[CompSales_Store_PreviousY3] [nvarchar](25) NULL,
	[CompSales_Store_PreviousY4] [nvarchar](25) NULL,
	[CompGC_CurrentYear] [nvarchar](25) NULL,
	[CompGC_Store_PreviousY1] [nvarchar](25) NULL,
	[CompGC_Store_PreviousY2] [nvarchar](25) NULL,
	[CompGC_Store_PreviousY3] [nvarchar](25) NULL,
	[CompGC_Store_PreviousY4] [nvarchar](25) NULL,
	[PS_TTM] [nvarchar](25) NULL,
	[SOI_TTM] [nvarchar](25) NULL,
	[SOIPct_TTM] [nvarchar](25) NULL,
	[SOIPct_PreviousY1] [nvarchar](25) NULL,
	[SOIPct_PreviousY3] [nvarchar](25) NULL,
	[SOIPct_PreviousY4] [nvarchar](25) NULL,
	[Margin_TTM] [nvarchar](25) NULL,
	[Margin_PreviousY1] [nvarchar](25) NULL,
	[Margin_PreviousY2] [nvarchar](25) NULL,
	[Margin_PreviousY3] [nvarchar](25) NULL,
	[Margin_PreviousY4] [nvarchar](25) NULL,
	[MarginPct_TTM] [nvarchar](25) NULL,
	[MarginPct_PreviousY1] [nvarchar](25) NULL,
	[MarginPct_PreviousY2] [nvarchar](25) NULL,
	[MarginPct_PreviousY3] [nvarchar](25) NULL,
	[MarginPct_PreviousY4] [nvarchar](25) NULL,
	[CF_TTM] [nvarchar](25) NULL,
	[CF_PreviousY1] [nvarchar](25) NULL,
	[CF_PreviousY2] [nvarchar](25) NULL,
	[CF_PreviousY3] [nvarchar](25) NULL,
	[CF_PreviousY4] [nvarchar](25) NULL,
	[Kiosk_Operation_Month] [nvarchar](25) NULL,
	[Kiosk_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[Kiosk_SalesPct] [nvarchar](25) NULL,
	[MDS_Operation_Month] [nvarchar](25) NULL,
	[MDS_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[MDS_SalesPct] [nvarchar](25) NULL,
	[McCafe_Operation_Month] [nvarchar](25) NULL,
	[McCafe_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[McCafe_SalesPct] [nvarchar](25) NULL,
	[H24_Operation_Month] [nvarchar](25) NULL,
	[H24_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[H24_SalesPct] [nvarchar](25) NULL,
	[DT_Operation_Month] [nvarchar](25) NULL,
	[DT_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[DT_SalesPct] [nvarchar](25) NULL,
	[Attached_Kiosk_Operation_Month] [nvarchar](25) NULL,
	[Attached_Kiosk_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[Attached_Kiosk_SalesPct] [nvarchar](25) NULL,
	[Remote_Kiosk_Operation_Month] [nvarchar](25) NULL,
	[Remote_Kiosk_Net_Product_Sales_Per_Month] [nvarchar](25) NULL,
	[Remote_Kiosk_SalesPct] [nvarchar](25) NULL,
	[Original_LHI] [nvarchar](25) NULL,
	[Original_ESSD] [nvarchar](25) NULL,
	[NBV_RE_Cost] [nvarchar](25) NULL,
	[NBV_LHI] [nvarchar](25) NULL,
	[NBV_ESSD] [nvarchar](25) NULL,
	[Price_Tier] [nvarchar](25) NULL,
	[YTD_Paid_to_Landlard] [nvarchar](25) NULL,
	[Actual_Paid_to_Landlard_PreviousY1] [nvarchar](25) NULL,
	[Actual_Paid_to_Landlard_PreviousY2] [nvarchar](25) NULL,
	[Actual_Paid_to_Landlard_PreviousY3] [nvarchar](25) NULL,
	[Actual_Paid_to_Landlard_PreviousY4] [nvarchar](25) NULL,
	[Lease_Adjustment_TTM] [nvarchar](25) NULL,
	[Lease_AdjustmentPct_TTM] [nvarchar](25) NULL,
	[Pac_TTM] [nvarchar](25) NULL,
	[Rent_TTM] [nvarchar](25) NULL,
	[RentPct_TTM] [nvarchar](25) NULL,
	[Depreciation_LHI_TTM] [nvarchar](25) NULL,
	[Interest_LHI_TTM] [nvarchar](25) NULL,
	[Service_Fee_TTM] [nvarchar](25) NULL,
	[Accounting_TTM] [nvarchar](25) NULL,
	[Insurance_TTM] [nvarchar](25) NULL,
	[Taxes_Licenses_TTM] [nvarchar](25) NULL,
	[Depreciation_Essd_TTM] [nvarchar](25) NULL,
	[Interest_Essd_TTM] [nvarchar](25) NULL,
	[Other_Exp_TTM] [nvarchar](25) NULL,
	[Non_Product_Sales_TTM] [nvarchar](25) NULL,
	[Non_Product_Costs_TTM] [nvarchar](25) NULL,
	[CompSalesPct_TTM] [nvarchar](25) NULL,
	[CompSalesPct_Market_TTM] [nvarchar](25) NULL,
	[CompGCPct_TTM] [nvarchar](25) NULL,
	[CompGCPct_Market_TTM] [nvarchar](25) NULL,
	[PAC_Pct_TTM] [nvarchar](25) NULL,
	[PAC_Pct_Market_TTM] [nvarchar](25) NULL,
	[SOI_Pct_Market_TTM] [nvarchar](25) NULL,
	[CashFlow_TTM] [nvarchar](25) NULL,
	[TotalSales_TTMPreviousY1] [nvarchar](25) NULL,
	[CompSalesPct_TTMPreviousY1] [nvarchar](25) NULL,
	[CompSalesPct_Market_TTMPreviousY1] [nvarchar](25) NULL,
	[CompGC_Pct_TTMPreviousY1] [nvarchar](25) NULL,
	[CompGC_Pct_Market_TTMPreviousY1] [nvarchar](25) NULL,
	[PAC_Pct_TTMPreviousY1] [nvarchar](25) NULL,
	[PAC_Pct_Market_TTMPreviousY1] [nvarchar](25) NULL,
	[SOI_Pct_TTMPreviousY1] [nvarchar](25) NULL,
	[SOI_Pct_Market_TTMPreviousY1] [nvarchar](25) NULL,
	[CashFlow_TTMPreviousY1] [nvarchar](25) NULL,
	[TotalSales_TTMPreviousY2] [nvarchar](25) NULL,
	[CompSales_Pct_TTMPreviousY2] [nvarchar](25) NULL,
	[CompSales_Pct_Market_TTMPreviousY2] [nvarchar](25) NULL,
	[CompGC_Pct_TTMPreviousY2] [nvarchar](25) NULL,
	[CompGC_Pct_Market_TTMPreviousY2] [nvarchar](25) NULL,
	[PAC_Pct_TTMPreviousY2] [nvarchar](25) NULL,
	[PAC_Pct_Market_TTMPreviousY2] [nvarchar](25) NULL,
	[SOI_Pct_TTMPreviousY2] [nvarchar](25) NULL,
	[SOI_Pct_Market_TTMPreviousY2] [nvarchar](25) NULL,
	[CashFlow_TTMPreviousY2] [nvarchar](25) NULL,
	[CreatedTime] [datetime] NULL,
 CONSTRAINT [PK_FA_Data] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅编号 US Code' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'StoreCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total Sales_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Total_Sales_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Net Product Sales_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Net_Product_Sales_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'GC_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'GC_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total_Sales_Y1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Total_Sales_Y1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Y2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Y2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Y3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Y3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Y4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Y4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompGC_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Current Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_CurrentYear'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Store_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Previous2Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Store_PreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Store_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompSales_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Store_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompGC_Current Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_CurrentYear'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompGC_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Store_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompGC_Previous2Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Store_PreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompGC_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Store_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CompGC_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Store_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PS_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PS_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOI_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI%_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOIPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI%_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOIPct_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI%_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOIPct_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI%_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOIPct_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Margin_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Margin_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin_Previous2Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Margin_PreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Margin_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Margin_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin%_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MarginPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin%_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MarginPct_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin%_Previous2Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MarginPct_PreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin%_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MarginPct_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Margin%_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MarginPct_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CF_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CF_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CF_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CF_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CF_Previous2Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CF_PreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CF_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CF_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CF_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CF_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kiosk_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Kiosk_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kiosk_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Kiosk_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kiosk_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Kiosk_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MDS_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MDS_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MDS_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MDS_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MDS_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'MDS_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'McCafe_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'McCafe_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'McCafe_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'McCafe_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'McCafe_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'McCafe_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'H24_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'H24_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'H24_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'H24_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'H24_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'H24_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DT_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'DT_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DT_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'DT_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DT_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'DT_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Attached_Kiosk_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Attached_Kiosk_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Attached_Kiosk_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Attached_Kiosk_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Attached_Kiosk_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Attached_Kiosk_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Remote_Kiosk_Operation_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Remote_Kiosk_Operation_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Remote_Kiosk_Net_Product_Sales_Per_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Remote_Kiosk_Net_Product_Sales_Per_Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Remote_Kiosk_SalesPct' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Remote_Kiosk_SalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Original LHI' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Original_LHI'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Original ESSD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Original_ESSD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'NBV RE Cost' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'NBV_RE_Cost'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'NBV LHI' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'NBV_LHI'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'NBV ESSD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'NBV_ESSD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Price Tier' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Price_Tier'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'YTD Paid to Landlard' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'YTD_Paid_to_Landlard'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Actual Paid to Landlard_Previous1Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Actual_Paid_to_Landlard_PreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Actual Paid to Landlard_Previous2Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Actual_Paid_to_Landlard_PreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Actual Paid to Landlard_Previous3Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Actual_Paid_to_Landlard_PreviousY3'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Actual Paid to Landlard_Previous4Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Actual_Paid_to_Landlard_PreviousY4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TTM Lease Adjustment' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Lease_Adjustment_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TTM Lease Adjustment %' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Lease_AdjustmentPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pac_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Pac_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Rent _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Rent_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Rent %_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'RentPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Depreciation LHI_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Depreciation_LHI_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Interest LHI_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Interest_LHI_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Service Fee _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Service_Fee_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Accounting _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Accounting_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Insurance _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Insurance_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Taxes & Licenses _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Taxes_Licenses_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Depreciation Essd _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Depreciation_Essd_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Interest Essd _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Interest_Essd_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Other (Inc)/Exp _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Other_Exp_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Non-Product Sales _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Non_Product_Sales_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Non-Product Costs _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'Non_Product_Costs_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. Sales %_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSalesPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. Sales % Market_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSalesPct_Market_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. GC %_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGCPct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. GC % Market_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGCPct_Market_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PAC %_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PAC_Pct_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PAC % Market_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PAC_Pct_Market_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI % Market_TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOI_Pct_Market_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cash Flow _TTM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CashFlow_TTM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total Sales_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'TotalSales_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. Sales %_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSalesPct_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. Sales % Market_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSalesPct_Market_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. GC %_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Pct_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. GC % Market_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Pct_Market_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PAC %_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PAC_Pct_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PAC % Market_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PAC_Pct_Market_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI %_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOI_Pct_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI % Market_TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOI_Pct_Market_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cash Flow _TTMPreviousY1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CashFlow_TTMPreviousY1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total Sales_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'TotalSales_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. Sales %_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Pct_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. Sales % Market_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompSales_Pct_Market_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. GC %_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Pct_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comp. GC % Market_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CompGC_Pct_Market_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PAC %_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PAC_Pct_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PAC % Market_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'PAC_Pct_Market_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI %_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOI_Pct_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI % Market_TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'SOI_Pct_Market_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cash Flow _TTMPreviousY2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CashFlow_TTMPreviousY2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAData', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO


/****** Object:  Table [dbo].[FAHistory]    Script Date: 2014/6/30 21:17:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FAHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreCode] [nvarchar](20) NULL,
	[AnnualYear] [nvarchar](25) NULL,
	[CompsSalesPct] [nvarchar](25) NULL,
	[SOI] [nvarchar](25) NULL,
	[CashFlow] [nvarchar](25) NULL,
	[ActualRentalToLL] [nvarchar](25) NULL,
	[CreatedTime] [datetime] NULL,
 CONSTRAINT [PK_FA_History] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅编号 US Code' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'StoreCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'AnnualYear'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Comps Sales%' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'CompsSalesPct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SOI(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'SOI'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cash Flow(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'CashFlow'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Actual rental to LL(RMB)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'ActualRentalToLL'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FAHistory', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO


/****** Object:  Table [dbo].[ProjectIdsInfo]    Script Date: 2014/6/30 21:17:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProjectIdsInfo](
	[ID] [uniqueidentifier] NOT NULL,
	[ProjectCode] [nvarchar](50) NULL,
	[ProjectID] [int] NULL,
	[CreateTime] [datetime] NULL,
	[LastUpdateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[LastUpdateUserAccount] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProjectIDs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ProjectIdsInfo] ADD  CONSTRAINT [DF_ProjectIdsInfo_ID]  DEFAULT (newid()) FOR [ID]
GO


/****** Object:  Table [dbo].[Remind]    Script Date: 2014/6/30 21:18:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Remind](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[IsReaded] [bit] NULL,
	[RefId] [nvarchar](50) NULL,
	[RegisterCode] [nvarchar](50) NULL,
	[Content] [nvarchar](500) NULL,
	[ReceiverAccount] [nvarchar](50) NULL,
	[ReceiverName] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[SenderAccount] [nvarchar](50) NULL,
	[SenderName] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_Remind1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Remind] ADD  CONSTRAINT [DF_Remind_Id_1]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Remind] ADD  CONSTRAINT [DF_Remind_Title]  DEFAULT (newid()) FOR [Title]
GO

ALTER TABLE [dbo].[Remind] ADD  CONSTRAINT [DF_Remind1_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'Title'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否已读' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'IsReaded'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用的ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'RefId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'注册编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'RegisterCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'Content'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收者账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'ReceiverAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收者名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'ReceiverName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'CreateUserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'SenderAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'SenderName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Remind', @level2type=N'COLUMN',@level2name=N'Sequence'
GO


/****** Object:  Table [dbo].[RemindRegister]    Script Date: 2014/6/30 21:18:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RemindRegister](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[ModuleName] [nvarchar](50) NULL,
	[ModuleCode] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[CreateAccount] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_Remind] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RemindRegister] ADD  CONSTRAINT [DF_Remind_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[RemindRegister] ADD  CONSTRAINT [DF_RemindRegister_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[RemindRegister] ADD  CONSTRAINT [DF_Remind_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'Code'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模块名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'ModuleName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模块编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'ModuleCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'CreateAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RemindRegister', @level2type=N'COLUMN',@level2name=N'Sequence'
GO



/****** Object:  Table [dbo].[StoreBaseInfo]    Script Date: 2014/6/30 21:18:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StoreBaseInfo](
	[StoreID] [int] IDENTITY(1,1) NOT NULL,
	[StoreCode] [nvarchar](20) NULL,
	[StoreNameCn] [nvarchar](100) NULL,
	[StoreNameEn] [nvarchar](100) NULL,
	[RegionName] [nvarchar](500) NULL,
	[MarketName] [nvarchar](500) NULL,
	[CityName] [nvarchar](500) NULL,
	[DistrictName] [nvarchar](500) NULL,
	[MMName] [nvarchar](500) NULL,
	[TATypeName] [nvarchar](500) NULL,
	[StoreTypeName] [nvarchar](500) NULL,
	[PortfolioTypeName] [nvarchar](500) NULL,
	[DTTypeName] [nvarchar](500) NULL,
	[IsAlliance] [int] NULL,
	[AllianceName] [nvarchar](500) NULL,
	[TVMarket] [int] NULL,
	[IsBigLL] [int] NULL,
	[BigLLName] [nvarchar](500) NULL,
	[StoreStatus] [nvarchar](500) NULL,
	[OpenDate] [datetime] NULL,
	[CloseDate] [datetime] NULL,
	[ReImagingDate] [datetime] NULL,
	[AddressCn] [nvarchar](300) NULL,
	[AddressEn] [nvarchar](300) NULL,
	[AssetRepName] [nvarchar](50) NULL,
	[AssetRepAD] [nvarchar](20) NULL,
	[AMName] [nvarchar](50) NULL,
	[AMAD] [nvarchar](20) NULL,
	[PlannerName] [nvarchar](50) NULL,
	[PlannerAD] [nvarchar](20) NULL,
	[PMName] [nvarchar](50) NULL,
	[PMAD] [nvarchar](20) NULL,
	[CMName] [nvarchar](50) NULL,
	[CMAD] [nvarchar](20) NULL,
	[PlanningMgrName] [nvarchar](50) NULL,
	[PlanningMgrAD] [nvarchar](20) NULL,
	[RepName] [nvarchar](50) NULL,
	[RepAD] [nvarchar](20) NULL,
	[RepMgrName] [nvarchar](50) NULL,
	[RepMgrAD] [nvarchar](20) NULL,
	[VPGMName] [nvarchar](50) NULL,
	[VPGMAD] [nvarchar](20) NULL,
	[GMName] [nvarchar](50) NULL,
	[GMAD] [nvarchar](20) NULL,
	[DOName] [nvarchar](50) NULL,
	[DOAD] [nvarchar](20) NULL,
	[OMName] [nvarchar](50) NULL,
	[OMAD] [nvarchar](20) NULL,
	[OCName] [nvarchar](50) NULL,
	[OCAD] [nvarchar](20) NULL,
	[StoreMgrName] [nvarchar](50) NULL,
	[StoreMgrAD] [nvarchar](20) NULL,
	[Tel] [nvarchar](50) NULL,
	[Email] [nvarchar](200) NULL,
	[CreatedTime] [datetime] NULL,
	[LatestModifyTime] [datetime] NULL,
 CONSTRAINT [PK_STORE_BASE_INFO] PRIMARY KEY CLUSTERED 
(
	[StoreID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'美国编号 ,US Code' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中文名称 ,StoreName_CN ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreNameCn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'英文名称 ,StoreName_EN ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreNameEn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区域 ,Region ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'RegionName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'市场 ,Market ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'MarketName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市 ,City ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'CityName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区县' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'DistrictName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MM 名称 ,Mini Market Name ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'MMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商圈类型 ,TA Classification ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'TATypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅类别 ,Store Type ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreTypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投资组合 ,Portfolio Type ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PortfolioTypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DT 型号 ,DT Type ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'DTTypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否由联盟团队推荐' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'IsAlliance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联盟名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AllianceName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TV Market' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'TVMarket'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否属于联盟企业' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'IsBigLL'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联盟名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'BigLLName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Status' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开业日期 ,Open Date ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'OpenDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关店日期 ,Close Date   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'CloseDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ReImaging 日期 ,ReImaging Date ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'ReImagingDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中文地址 ,Address_Chinese ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AddressCn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'英文地址 ,Address_English ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AddressEn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资产专员（当前）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AssetRepName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资产专员AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AssetRepAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资产经理（当前）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资产经理AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'AMAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Planner(开业时)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PlannerName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Planner AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PlannerAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目经理(开业时)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目经理 AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PMAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工程经理(开业时)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'CMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工程经理 AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'CMAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'规划经理(开业时)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PlanningMgrName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'规划经理 AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'PlanningMgrAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地产代表(开业时)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'RepName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地产代表 AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'RepAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地产经理(开业时)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'RepMgrName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地产经理 AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'RepMgrAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'VPGM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'VPGMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'VPGM AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'VPGMAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'GM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'GMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'GM AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'GMAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DO' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'DOName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DO AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'DOAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'OM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'OMName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'OM AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'OMAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'OC' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'OCName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'OC AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'OCAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅经理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreMgrName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅经理 AD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'StoreMgrAD'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'Tel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'餐厅Email' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'Email'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreBaseInfo', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO



/****** Object:  Table [dbo].[TaskWork]    Script Date: 2014/6/30 21:18:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TaskWork](
	[Id] [uniqueidentifier] NOT NULL,
	[Num] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Url] [nvarchar](500) NULL,
	[SourceCode] [nvarchar](120) NULL,
	[SourceName] [nvarchar](120) NULL,
	[RefID] [nvarchar](50) NULL,
	[TypeCode] [nvarchar](50) NULL,
	[TypeName] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[StatusName] [nvarchar](50) NULL,
	[ReceiverAccount] [nvarchar](50) NULL,
	[ReceiverName] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[CreateUserAccount] [nvarchar](50) NULL,
	[FinishTime] [datetime] NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_Task_1] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TaskWork] ADD  CONSTRAINT [DF_Task_Guid]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[TaskWork] ADD  CONSTRAINT [DF_Task_Status]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[TaskWork] ADD  CONSTRAINT [DF_TaskWork_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'Num'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'Title'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'链接' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'Url'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'来源编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'SourceCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'来源名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'SourceName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用的ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'RefID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'TypeCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收者账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'ReceiverAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收者姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'ReceiverName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建者账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'CreateUserAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'完成时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'FinishTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskWork', @level2type=N'COLUMN',@level2name=N'Sequence'
GO


/****** Object:  Table [dbo].[TaskWorkRegister]    Script Date: 2014/6/30 21:18:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TaskWorkRegister](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[ModuleName] [nvarchar](50) NULL,
	[ModuleCode] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[CreateAccount] [nvarchar](50) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_TaskWorkRegister] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TaskWorkRegister] ADD  CONSTRAINT [DF_TaskWorkRegister_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[TaskWorkRegister] ADD  CONSTRAINT [DF_TaskWorkRegister_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[TaskWorkRegister] ADD  CONSTRAINT [DF_TaskWorkRegister_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO





