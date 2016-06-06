USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_DelProductByID')
BEGIN
	DROP PROCEDURE usp_DelProductByID
END
GO

CREATE PROCEDURE usp_DelProductByID
@ProductID nvarchar(30)
AS
BEGIN
	DELETE FROM tbl_Product WHERE ProductID = @ProductID
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to DELTE tbl_Product.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT StatusCode='0',StatusDesc='Success'
	END
END

