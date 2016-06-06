USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateProductByProductID')
BEGIN
	DROP PROCEDURE usp_AddUpdateProductByProductID
END
GO

CREATE PROCEDURE usp_AddUpdateProductByProductID
@ProductID nvarchar(30)=null,

@CategoryID nvarchar(30)=null,

@SupplierID nvarchar(30)=null,

@ProductName nvarchar(300)=null,
@ProductDesc nvarchar(max)=null,
@ProductSpec nvarchar(max)=null,

@ProductImgs nvarchar(max)=null,
@MoyeBuyPrice decimal=null,
@MarketPrice decimal=null,
@ProductCount int = 9999,
@IsSellHot bit =null,
@IsOnSell bit =null
AS
BEGIN
DECLARE @RandNum NVARCHAR(30)
DECLARE @RandNum1 NVARCHAR(30)
	
	IF @ProductID IS NOT NULL
	BEGIN
		SET @RandNum= @ProductID
		UPDATE tbl_Product SET 
		CategoryID =@CategoryID,
		ProductName = @ProductName,
		ProductDesc = @ProductDesc,
		ProductSpec = @ProductSpec,
		ProductImgs = @ProductImgs,
		MoyeBuyPrice = @MoyeBuyPrice,
		MarketPrice = @MarketPrice,
		IsSellHot =@IsSellHot,
		IsOnSell = @IsOnSell,
		LastUpdatedDate = GETDATE()
		WHERE ProductID = @ProductID
	END
	ELSE
	BEGIN
	BEGIN TRANSACTION
	
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	SET @RandNum='PRID'+@RandNum
	SET @RandNum1 ='PSID'+@RandNum
		INSERT INTO tbl_Product(
		ProductID,
		CategoryID,
		ProductStoreID,
		ProductName,
		ProductDesc,
		ProductSpec,
		ProductImgs,
		MoyeBuyPrice,
		MarketPrice,
		IsSellHot,
		IsOnSell,
		LastUpdatedDate)
		VALUES(
		@RandNum,
		@CategoryID,
		@RandNum1,
		@ProductName,
		@ProductDesc,
		@ProductSpec,
		@ProductImgs,
		@MoyeBuyPrice,
		@MarketPrice,
		@IsSellHot,
		@IsOnSell,
		GETDATE())
		
		IF(@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION
			SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
			RETURN 2
		END
		
		INSERT INTO tbl_ProductStore(
		ProductStoreID,
		ProductID,
		ProductCount,
		SupplierID,
		LastUpdatedDate)
		VALUES(
		@RandNum1,
		@RandNum,
		@ProductCount,
		@SupplierID,
		GETDATE()
		)
		
		IF(@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION
			SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
			RETURN 2
		END
	COMMIT TRANSACTION
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_Order.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT ProductID= @RandNum,StatusCode='0',StatusDesc='Success'
	END

END
