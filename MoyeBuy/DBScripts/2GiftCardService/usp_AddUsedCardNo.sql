USE [GiftCardService]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUsedCardNo')
BEGIN
	DROP PROCEDURE usp_AddUsedCardNo
END
GO

CREATE PROCEDURE usp_AddUsedCardNo
@GiftCardNo nvarchar(100),
@UID nvarchar(30)
AS
BEGIN
	DECLARE @RandNum NVARCHAR(30)
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	SET @RandNum='INVD'+@RandNum
	INSERT INTO tbl_GiftCardInvalid(
	GiftCardInvalidID,
	GiftCardNo,
	UpdateByUserID,
	LastUpdatedDate)
	VALUES(
	@RandNum,
	@GiftCardNo,
	@UID,
	GETDATE())
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_GiftCardInvalid.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT GiftCardInvalidID= @RandNum,StatusCode='0',StatusDesc='Success'
	END
	
END
