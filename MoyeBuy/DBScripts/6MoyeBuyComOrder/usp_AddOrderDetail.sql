USE [MoyeBuyComOrder]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddOrderDetail')
BEGIN
	DROP PROCEDURE usp_AddOrderDetail
END
GO

CREATE PROCEDURE usp_AddOrderDetail
@OrderID nvarchar(30) ,
@ProductID nvarchar(30),
@ProductCount int,
@UID nvarchar(30)
AS
BEGIN
	DECLARE @RandNum NVARCHAR(30)
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	set @RandNum='ODet'+@RandNum
	INSERT INTO tbl_OrderDetail(OrderDetailID,OrderID,ProductID,ProductCount,LastUpdatedDate,UpdateByUserID)
	VALUES(@RandNum,@OrderID,@ProductID,@ProductCount,GETDATE(),@UID)
		
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_OrderDetail.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT OrderDetailID = @RandNum,StatusCode='0',StatusDesc='Success'
		return 0
	END
END
GO