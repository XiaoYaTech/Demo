
-------MoyeBuyComLog
USE [MoyeBuyComLog];
IF Exists(select 1 from sys.all_objects where name='tbl_SystemLog')
BEGIN
DROP TABLE tbl_SystemLog;
END
Create Table tbl_SystemLog
(
	SystemLogID int primary key not null,
	SystemLogMsg nvarchar(max) null,
	SystemLogPosition nvarchar(500) null,
	UpdatedByUserID nvarchar(30) not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_SystemLog_UpdatedByUserID ON tbl_SystemLog(UpdatedByUserID)



-------------------------------
IF Exists(select 1 from sys.all_objects where name='tbl_UserOperatorTrack')
BEGIN
DROP TABLE tbl_UserOperatorTrack;
END
CREATE TABLE tbl_UserOperatorTrack
(
	UserOperatorTrackID nvarchar(30) primary key ,
	ParentUserOperatorTrackID nvarchar(30) null,
	UserID nvarchar(30) not null,
	FromURL nvarchar(500) null,
	PageName nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_UserOperatorTrack_UpdatedByUserID ON tbl_UserOperatorTrack(ParentUserOperatorTrackID)




-------MoyeBuyCom
USE [MoyeBuyCom];
IF Exists(select 1 from sys.all_objects where name='tbl_Product')
BEGIN
DROP TABLE tbl_Product;
END
Create Table tbl_Product
(
	ProductID nvarchar(30) primary key not null,
	CategoryID nvarchar(30) not null,
	--CommentID nvarchar(30) not null,
	ProductStoreID nvarchar(30) not null,
	ProductName nvarchar(300) not null,
	ProductDesc nvarchar(max) null,
	ProductSpec nvarchar(max) null,
	ProductImgs nvarchar(max) null,
	MoyeBuyPrice decimal(10,2) not null,
	MarketPrice decimal(10,2) not null,
	IsSellHot bit DEFAULT(0) not null,
	IsOnSell bit DEFAULT(1) not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_Product_CategoryID ON  tbl_Product(CategoryID)
CREATE INDEX index_tbl_Product_ProductName ON  tbl_Product(ProductName)


IF Exists(select 1 from sys.all_objects where name='tbl_ProductStore')
BEGIN
DROP TABLE tbl_ProductStore;
END
Create Table tbl_ProductStore
(
	ProductStoreID nvarchar(30) primary key not null,
	ProductID nvarchar(30) not null,
	ProductCount int not null,
	SupplierID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_ProductStore_ProductID ON tbl_ProductStore(ProductID)
CREATE INDEX index_tbl_ProductStore_SupplierID ON tbl_ProductStore(SupplierID)


IF Exists(select 1 from sys.all_objects where name='tbl_Supplier')
BEGIN
DROP TABLE tbl_Supplier;
END
Create table tbl_Supplier
(
	SupplierID nvarchar(30) primary key not null,
	SupplierName nvarchar(300) unique not null,
	SupplierPersonName nvarchar(30) null,
	SupplierPhoneNo nvarchar(30) null,
	SupplierFax nvarchar(30) null,
	SupplierAddress nvarchar(300) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_Comment_SupplierName ON  tbl_Supplier(SupplierName)


IF Exists(select 1 from sys.all_objects where name='tbl_Comment')
BEGIN
DROP TABLE tbl_Comment;
END
Create table tbl_Comment
(
	CommentID nvarchar(30) primary key not null,
	ParentCommentID nvarchar(30) null,
	UserID nvarchar(30) not null,
	ProductID nvarchar(30) not null,
	CommentCatgoryID int not null,
	CommentDesc nvarchar(1000) null,
	CommentState nvarchar(10) null,
	CommentAttitude nvarchar(10) null,
	IsAgree bit null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_Comment_CommentCatgoryID ON  tbl_Comment(CommentCatgoryID)
CREATE INDEX index_tbl_Comment_UserID ON  tbl_Comment(UserID)


IF Exists(select 1 from sys.all_objects where name='tbl_CommentCatgory')
BEGIN
DROP TABLE tbl_CommentCatgory;
END
CREATE TABLE tbl_CommentCatgory
(
	CommentCatgoryID int primary key identity(1,1) not null ,
	CommentCatgoryName nvarchar(30) not null,--满意,不满意，一般,讨论
	LastUpdatedDate datetime DEFAULT(GETDATE())
)

IF Exists(select 1 from sys.all_objects where name='tbl_ProductCategory')
BEGIN
DROP TABLE tbl_ProductCategory;
END
Create table tbl_ProductCategory
(
	CategoryID nvarchar(30) primary key not null,
	CategoryName nvarchar(30) not null,
	CategoryDesc nvarchar(300) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_ProductCategory_CategoryName ON  tbl_ProductCategory(CategoryName)

-------------------------------tbl_LayoutMenu
IF Exists(select 1 from sys.all_objects where name='tbl_LayoutMenu')
BEGIN
DROP TABLE tbl_LayoutMenu;
END
CREATE TABLE tbl_LayoutMenu
(
	MenuID int primary key identity(1,1),
	MenuType nvarchar(50) not null,
	IsAdminMenu bit not null,
	MenuName nvarchar(50) null,
	MenuURL nvarchar(300) null,
	MenuDisq int not null,
	MenuTarget nvarchar(10) null,
	MenuClassName nvarchar(50) null,
	MenuControlID nvarchar(50) null,
	MenuTitle nvarchar(50) null,
	LastUpdatedDate Datetime default(getdate())
)

-------------------------------tbl_LayoutSubMenuMapping
IF Exists(select 1 from sys.all_objects where name='tbl_LayoutSubMenuMapping')
BEGIN
DROP TABLE tbl_LayoutSubMenuMapping;
END
CREATE TABLE tbl_LayoutSubMenuMapping
(
	SubMenuMappingID int primary key identity(1,1),
	MenuID int not null,
	SubMenuID int not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
Alter table tbl_LayoutSubMenuMapping add constraint FK_MenuID_tbl_LayoutMenu foreign key(MenuID) references tbl_LayoutMenu(MenuID)
Alter table tbl_LayoutSubMenuMapping add constraint FK_SubMenuId_tbl_LayoutMenu foreign key(SubMenuId) references tbl_LayoutMenu(MenuID)

-------------------------------tbl_Advertisement
IF Exists(select 1 from sys.all_objects where name='tbl_Advertisement')
BEGIN
DROP TABLE tbl_Advertisement;
END
CREATE TABLE tbl_Advertisement
(
	AdID int primary key identity(1,1),
	AdTitle nvarchar(20) null,
	AdType nvarchar(20) not null,
	AdImgs nvarchar(max) null,
	AdImgAltTitle nvarchar(max) null,
	AdImgDisq nvarchar(max) null,
	AdImigDesc nvarchar(max) null, 
	AdUrl nvarchar(max) null,
	AdTarget nvarchar(max) null,
	AdClassName nvarchar(max) null,
	AdControlID nvarchar(max) null,
)

----------MoyeBuyComOrder
USE [MoyeBuyComOrder];
IF Exists(select 1 from sys.all_objects where name='tbl_Address')
BEGIN
DROP TABLE tbl_Address;
END
Create table tbl_Address
(
	AddressID nvarchar(30) primary key not null,
	Name nvarchar(20) not null,
	AddressLabel nvarchar(20) not null,
	Province nvarchar(50) not null,
	--[State] nvarchar(20) null,
	City nvarchar(50) null,
	District nvarchar(50) null,
	AddressDetail nvarchar(100) null,
	ZipCode int null,
	MobilePhone nvarchar(30) null,
	TelPhone nvarchar(30) null,
	UpdateByUserID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)

IF Exists(select 1 from sys.all_objects where name='tbl_Order')
BEGIN
DROP TABLE tbl_Order;
END
Create table tbl_Order
(
	OrderID nvarchar(30) primary key not null,
	OrderDate datetime not null,
	UserID nvarchar(30) not null,
	AddressID nvarchar(30) not null,
	OrderTotal decimal(10,2) null,
	OrderStateID int null,
	ShippingModeID int null,
	ShippingTime Datetime null,
	IsPayed bit null,
	PayBank nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_Order_OrderStateID ON tbl_Order(OrderStateID)
CREATE INDEX index_tbl_Order_UserID ON tbl_Order(UserID)


IF Exists(select 1 from sys.all_objects where name='tbl_OrderDetail')
BEGIN
DROP TABLE tbl_OrderDetail;
END
Create table tbl_OrderDetail
(
	OrderDetailID nvarchar(30) primary key not null,
	OrderID nvarchar(30) not null,
	ProductID nvarchar(30) not null,
	ProductCount int not null,
	UpdateByUserID nvarchar(30) not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_OrderDetail_OrderID ON tbl_OrderDetail(OrderID)
CREATE INDEX index_tbl_OrderDetail_LastUpdatedBy ON tbl_OrderDetail(UpdateByUserID)



IF Exists(select 1 from sys.all_objects where name='tbl_OrderState')
BEGIN
DROP TABLE tbl_OrderState;
END
Create table tbl_OrderState
(
	OrderStateID int primary key identity(1,1) not null,
	OrderStateDesc nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)


IF Exists(select 1 from sys.all_objects where name='tbl_OrderTrack')
BEGIN
DROP TABLE tbl_OrderTrack;
END
CREATE TABLE tbl_OrderTrack
(
	OrderTrackID int primary key identity(1,1),
	OrderTrackStatus nvarchar(30) not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)


IF Exists(select 1 from sys.all_objects where name='tbl_OrderTrackDetail')
BEGIN
DROP TABLE tbl_OrderTrackDetail;
END
CREATE TABLE tbl_OrderTrackDetail
(
	OrderTrackDetailID nvarchar(30) primary key,
	OrderID nvarchar(30) not null,
	UpdateByUserID nvarchar(30) not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_OrderTrackDetail_OrderID ON tbl_OrderTrackDetail(OrderID)
CREATE INDEX index_tbl_OrderTrackDetail_LastUpdatedBy ON tbl_OrderTrackDetail(UpdateByUserID)



IF Exists(select 1 from sys.all_objects where name='tbl_ShippingMode')
BEGIN
DROP TABLE tbl_ShippingMode;
END
Create table tbl_ShippingMode
(
	ShippingModeID int primary key identity(1,1) not null,
	ShippingModeDesc nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)

---------MoyeBuyComServices
USE [MoyeBuyComServices]
IF Exists(select 1 from sys.all_objects where name='tbl_MoyeBuyComUser')
BEGIN
DROP TABLE tbl_MoyeBuyComUser;
END
CREATE TABLE tbl_MoyeBuyComUser
(
	MoyeBuyComUserID nvarchar(30) primary key ,
	AddressID nvarchar(30) null,
	MoyeBuyComUserName nvarchar(12) null,
	UserPhoneNo  nvarchar(30) null,
	MoyeBuyComEmail nvarchar(50) unique not null ,
	MoyeBuyComPwdSalt nvarchar(30) not null,
	MoyeBuyComPwdHash nvarchar(100) not null,
	IsEffective bit not null,
	IsNeedChangePwd bit null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_MoyeBuyComEmail ON tbl_MoyeBuyComUser(MoyeBuyComEmail)


------------tbl_Role
USE [MoyeBuyComServices]
IF Exists(select 1 from sys.all_objects where name='tbl_Role')
BEGIN
DROP TABLE tbl_Role;
END
CREATE TABLE tbl_Role
(
	RoleID int identity(1,1) primary key ,
	RoleName nvarchar(30) unique not null,
	RoleDesc nvarchar(100) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)

------------tbl_UserRole
USE [MoyeBuyComServices]
IF Exists(select 1 from sys.all_objects where name='tbl_UserRole')
BEGIN
DROP TABLE tbl_UserRole;
END
CREATE TABLE tbl_UserRole
(
	UserRoleID int identity(1,1) primary key ,
	MoyeBuyComUserID nvarchar(30) not null,
	RoleID int not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)



----------GiftCardService
USE [GiftCardService]
IF Exists(select 1 from sys.all_objects where name='tbl_GiftCard')
BEGIN
DROP TABLE tbl_GiftCard;
END
CREATE TABLE tbl_GiftCard
(
	GiftCardNoID nvarchar(30) primary key,
	GiftCardNo nvarchar(100) unique not null,
	--GiftCardPwdSalt nvarchar(50) not null,
	GifCardPwdHash varbinary(max) not null,
	GiftCardAmount decimal not null,
	UpdateByUserID nvarchar(30) not null,
	StartDate datetime not null,
	[ExpireDate] datetime not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_GiftCard_GiftCardNo ON tbl_GiftCard(GiftCardNo)

IF Exists(select 1 from sys.all_objects where name='tbl_GiftCardInvalid')
BEGIN
DROP TABLE tbl_GiftCardInvalid;
END
CREATE TABLE tbl_GiftCardInvalid
(
	GiftCardInvalidID nvarchar(30) primary key,
	GiftCardNo nvarchar(100) not null,
	UpdateByUserID nvarchar(30) not null,
	LastUpdatedDate datetime not null
)
CREATE INDEX index_ttbl_GiftCardInvalid_GiftCardNo ON tbl_GiftCardInvalid(GiftCardNo)
CREATE INDEX index_ttbl_GiftCardInvalid_UpdateByUserID ON tbl_GiftCardInvalid(UpdateByUserID)

------tbl_Product
USE [MoyeBuyCom]
Alter table tbl_Product add constraint FK_CategoryID_tbl_ProductCategory foreign key(CategoryID) references tbl_ProductCategory(CategoryID)
Alter table tbl_Product add constraint FK_CommentId_tbl_Comment foreign key(CommentID) references tbl_Comment(CommentID)
Alter table tbl_Product add constraint FK_ProductStoreId_tbl_ProductStore foreign key(ProductStoreID) references tbl_ProductStore(ProductStoreID)

------tbl_Comment
Alter table tbl_Comment add constraint FK_CommentCatgoryId_tbl_Catgory foreign key(CommentCatgoryID) references tbl_CommentCatgory(CommentCatgoryID)