USE [MoyeBuyComOrder]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetOrderDetailInfoByOrderID')
BEGIN
	DROP PROCEDURE usp_GetOrderDetailInfoByOrderID
END
GO

CREATE PROCEDURE usp_GetOrderDetailInfoByOrderID
@OrderID nvarchar(30),
@UID nvarchar(30)
AS
BEGIN
	SELECT o.*,
	dt.ProductCount,
	dt.ProductID,
	dt.OrderDetailID,
	p.ProductName,
	p.ProductDesc,
	p.ProductImgs,
	p.ProductSpec,
	p.MoyeBuyPrice,
	os.OrderStateDesc,
	ad.AddressLabel,
	ad.City,
	ad.Province,
	ad.District,
	ad.AddressDetail,
	ad.Name,
	ad.MobilePhone,
	ad.TelPhone,
	spm.ShippingModeDesc
	 FROM tbl_Order o INNER JOIN tbl_OrderDetail dt ON o.OrderID = dt.OrderID 
		INNER JOIN MoyeBuyCom.dbo.tbl_Product p ON dt.ProductID = p.ProductID
		INNER JOIN tbl_OrderState os ON o.OrderStateID = os.OrderStateID
		INNER JOIN tbl_Address ad ON o.AddressID = ad.AddressID
		INNER JOIN tbl_ShippingMode spm ON o.ShippingModeID = spm.ShippingModeID
	WHERE  o.UserID=@UID AND o.OrderID = @OrderID
END