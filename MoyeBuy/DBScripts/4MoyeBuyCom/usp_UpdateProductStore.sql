USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_UpdateProductComment')
BEGIN
	DROP PROCEDURE usp_UpdateProductStore
END
GO

CREATE PROCEDURE usp_UpdateProductStore
@ProductID nvarchar(30),
@ProductCount int ,
@SupplierID nvarchar(30)=null
AS
BEGIN
	UPDATE tbl_ProductStore SET 
	ProductCount=@ProductCount,
	SupplierID = @SupplierID,
	LastUpdatedDate = GETDATE()
	WHERE ProductID = @ProductID
END