USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetProductStoreByProductIDSupplierID')
BEGIN
	DROP PROCEDURE usp_GetProductStoreByProductIDSupplierID
END
GO

CREATE PROCEDURE usp_GetProductStoreByProductIDSupplierID
@ProductID nvarchar(30)=null,
@SupplierID nvarchar(30)=null
AS
BEGIN
	SELECT * FROM tbl_ProductStore WHERE ProductID=ISNULL(@ProductID,ProductID)
END