USE [MoyeBuyComOrder]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateOrderByOrderID')
BEGIN
	DROP PROCEDURE usp_AddUpdateOrderByOrderID
END
GO


CREATE PROCEDURE usp_AddUpdateOrderByOrderID
@OrderID nvarchar(30) = null,
@OrderDate datetime =null,
@UserID nvarchar(30)=null,
@AddressID nvarchar(30)=null,
@OrderTotal decimal =null,
@OrderStateID int =null,
@ShippingModeID int =null,
@ShippingTime datetime =null,
@IsPayed bit =null,
@PayBank nvarchar(30)=null,
@ProductIDs nvarchar(max),
@ProductCounts nvarchar(max)
AS
BEGIN
DECLARE @ProductID nvarchar(30)
DECLARE @iCount_ProductID int
DECLARE @ilength_ProductID int

DECLARE @ProductCount int
DECLARE @iCount_ProductCount int
DECLARE @ilength_ProductCount int
DECLARE @RandNum NVARCHAR(30)

IF(EXISTS(SELECT 1 FROM tbl_Order WHERE OrderID=@OrderID))
BEGIN 
BEGIN TRANSACTION
	SET @RandNum = @OrderID
	UPDATE tbl_Order SET OrderDate=@OrderDate,
	UserID=@UserID,
	AddressID=@AddressID,
	OrderTotal=@OrderTotal,
	OrderStateID=@OrderStateID,
	ShippingModeID=@ShippingModeID,
	ShippingTime=@ShippingTime,
	IsPayed=@IsPayed,
	PayBank=@PayBank,
	LastUpdatedDate=GETDATE() 
	WHERE OrderID=@OrderID
			
	IF(@@ERROR <> 0)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
		RETURN 2
	END
			
	IF @ProductIDs IS NOT NULL
	AND @ProductCounts IS NOT NULL
	BEGIN
		SET @iCount_ProductID  = 1
		SET @iCount_ProductCount  = 1
		WHILE @iCount_ProductID <>0
			AND @iCount_ProductCount <>0
		BEGIN
			SET @iCount_ProductID  = PATINDEX('%|||%',@ProductIDs)
			SET @ilength_ProductID  = CASE @iCount_ProductID WHEN 0 THEN DATALENGTH(@ProductIDs) ELSE @iCount_ProductID - 1 END
			SET @ProductID = NULLIF(LTRIM(SUBSTRING(@ProductIDs, 1, @ilength_ProductID)),'')
			
			SET @iCount_ProductCount  = PATINDEX('%|||%',@ProductCounts)
			SET @ilength_ProductCount  = CASE @iCount_ProductCount WHEN 0 THEN DATALENGTH(@ProductCounts) ELSE @iCount_ProductCount - 1 END
			SET @ProductCount = NULLIF(LTRIM(SUBSTRING(@ProductCounts, 1, @ilength_ProductCount)),'')
			
			--print @ProductID+','
			--print CONVERT(NVARCHAR(10),@ProductCount)
			
			UPDATE tbl_OrderDetail SET ProductID=@ProductID,
			ProductCount=@ProductCount,
			UpdateByUserID=@UserID,
			LastUpdatedDate=GETDATE()
			WHERE OrderID=@OrderID
			
			IF(@@ERROR <> 0)
			BEGIN
				ROLLBACK TRANSACTION
				SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
				RETURN 2
			END
	
			SET @ProductIDs = SUBSTRING(@ProductIDs,@iCount_ProductID + 3,DATALENGTH(@ProductIDs))
			SET @ProductCounts = SUBSTRING(@ProductCounts,@iCount_ProductCount + 3,DATALENGTH(@ProductCounts))
		END
	END
COMMIT TRANSACTION
END 
ELSE
BEGIN
BEGIN TRANSACTION
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	set @RandNum='OdID'+@RandNum
	INSERT INTO tbl_Order(OrderID,OrderDate,UserID,AddressID,OrderTotal,OrderStateID,ShippingModeID,ShippingTime,IsPayed,PayBank,LastUpdatedDate)
	VALUES(@RandNum,@OrderDate,@UserID,@AddressID,@OrderTotal,@OrderStateID,@ShippingModeID,@ShippingTime,@IsPayed,@PayBank,GETDATE())
	
	IF(@@ERROR <> 0)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
		RETURN 2
	END
		
	IF @ProductIDs IS NOT NULL AND @ProductCounts IS NOT NULL
	BEGIN
		SET @iCount_ProductID  = 1
		SET @iCount_ProductCount  = 1
		WHILE @iCount_ProductID <>0
			AND @iCount_ProductCount <>0
		BEGIN
			SET @iCount_ProductID  = PATINDEX('%|||%',@ProductIDs)
			SET @ilength_ProductID  = CASE @iCount_ProductID WHEN 0 THEN DATALENGTH(@ProductIDs) ELSE @iCount_ProductID - 1 END
			SET @ProductID = NULLIF(LTRIM(SUBSTRING(@ProductIDs, 1, @ilength_ProductID)),'')
			
			SET @iCount_ProductCount  = PATINDEX('%|||%',@ProductCounts)
			SET @ilength_ProductCount  = CASE @iCount_ProductCount WHEN 0 THEN DATALENGTH(@ProductCounts) ELSE @iCount_ProductCount - 1 END
			SET @ProductCount = NULLIF(LTRIM(SUBSTRING(@ProductCounts, 1, @ilength_ProductCount)),'')
			
			--print @ProductID+','
			--print CONVERT(NVARCHAR(10),@ProductCount)
			
			EXEC usp_AddOrderDetail @OrderID=@RandNum,@ProductID=@ProductID,@ProductCount=@ProductCount,@UID=@UserID

			IF(@@ERROR <> 0)
			BEGIN
				ROLLBACK TRANSACTION
				SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
				RETURN 2
			END
	
			SET @ProductIDs = SUBSTRING(@ProductIDs,@iCount_ProductID + 3,DATALENGTH(@ProductIDs))
			SET @ProductCounts = SUBSTRING(@ProductCounts,@iCount_ProductCount + 3,DATALENGTH(@ProductCounts))
		END
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
	SELECT OrderID= @RandNum,StatusCode='0',StatusDesc='Success'
END

END
	


