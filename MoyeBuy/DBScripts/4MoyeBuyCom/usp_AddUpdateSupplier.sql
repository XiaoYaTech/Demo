USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateSupplier')
BEGIN
	DROP PROCEDURE usp_AddUpdateSupplier
END
GO

CREATE PROCEDURE usp_AddUpdateSupplier
@SupplierID nvarchar(30) = null,
@SupplierName nvarchar(300)= null,
@SupplierPersonName nvarchar(30)= null,
@SupplierPhoneNo nvarchar(30)= null,
@SupplierFax nvarchar(30)= null,
@SupplierAddress nvarchar(300)= null
AS
BEGIN
DECLARE @RandNum NVARCHAR(30)
	IF @SupplierID IS NOT NULL
	BEGIN
		SET @RandNum = @SupplierID
		UPDATE tbl_Supplier SET 
		SupplierName = @SupplierName,
		SupplierPersonName = @SupplierPersonName,
		SupplierPhoneNo = @SupplierPhoneNo,
		SupplierFax = @SupplierFax,
		SupplierAddress = @SupplierAddress,
		LastUpdatedDate = GETDATE()
		WHERE SupplierID =@SupplierID
	END
	ELSE
	BEGIN
		EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
		SET @RandNum='Supp'+@RandNum
		INSERT INTO tbl_Supplier(
		SupplierID,
		SupplierName,
		SupplierPersonName,
		SupplierPhoneNo,
		SupplierFax,
		SupplierAddress,
		LastUpdatedDate)
		VALUES(
		@RandNum,
		@SupplierName,
		@SupplierPersonName,
		@SupplierPhoneNo,
		@SupplierFax,
		@SupplierAddress,
		GETDATE()
		)
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_Supplier.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT SupplierID= @RandNum,StatusCode='0',StatusDesc='Success'
	END
	
END

