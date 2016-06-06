USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetSupplierBySupplierIDName')
BEGIN
	DROP PROCEDURE usp_GetSupplierBySupplierIDName
END
GO

CREATE PROCEDURE usp_GetSupplierBySupplierIDName
@SupplierName nvarchar(30)=null,
@SupplierID nvarchar(30)=null
AS
BEGIN
	SELECT * FROM tbl_Supplier WHERE SupplierName=ISNULL(@SupplierName,SupplierName) 
	AND SupplierID = ISNULL(@SupplierID,SupplierID)
END