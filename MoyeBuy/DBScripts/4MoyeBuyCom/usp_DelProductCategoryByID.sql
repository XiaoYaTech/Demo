USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_DelProductCategoryByID')
BEGIN
	DROP PROCEDURE usp_DelProductCategoryByID
END
GO

CREATE PROCEDURE usp_DelProductCategoryByID
@CategoryID nvarchar(30)
AS
BEGIN
	DELETE FROM tbl_ProductCategory WHERE CategoryID = @CategoryID
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to DELTE tbl_ProductCategory.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT StatusCode='0',StatusDesc='Success'
	END
END
