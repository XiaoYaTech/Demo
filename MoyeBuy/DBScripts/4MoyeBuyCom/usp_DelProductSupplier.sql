USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_DelProductSupplier')
BEGIN
	DROP PROCEDURE usp_DelProductSupplier
END
GO

CREATE PROCEDURE usp_DelProductSupplier
@SupplierID nvarchar(30)
AS
BEGIN
	DELETE FROM tbl_Supplier WHERE SupplierID = @SupplierID
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to DELTE tbl_Supplier.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT StatusCode='0',StatusDesc='Success'
	END
END

