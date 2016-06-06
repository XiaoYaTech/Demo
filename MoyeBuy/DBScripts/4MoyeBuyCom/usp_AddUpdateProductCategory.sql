USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateProductCategory')
BEGIN
	DROP PROCEDURE usp_AddUpdateProductCategory
END
GO

CREATE PROCEDURE usp_AddUpdateProductCategory
@CategoryID nvarchar(30)=null,
@CategoryName nvarchar(30),
@CategoryDesc nvarchar(300)
AS
BEGIN
DECLARE @RandNum NVARCHAR(30)
	IF @CategoryID IS NOT NULL
	BEGIN
		UPDATE tbl_ProductCategory SET CategoryName=@CategoryName,CategoryDesc=@CategoryDesc,LastUpdatedDate=GETDATE()
		WHERE CategoryID =@CategoryID
	END
	ELSE
	BEGIN
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	SET @RandNum='CTID'+@RandNum
		INSERT INTO tbl_ProductCategory(
		CategoryID,
		CategoryName,
		CategoryDesc,
		LastUpdatedDate)
		VALUES(
		@RandNum,
		@CategoryName,
		@CategoryDesc,
		GETDATE()
		)
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_ProductCategory.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT CategoryID= @RandNum,StatusCode='0',StatusDesc='Success'
	END
END