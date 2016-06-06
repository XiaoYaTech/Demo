USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateAritcleType')
BEGIN
	DROP PROCEDURE usp_AddUpdateAritcleType
END
GO

CREATE PROCEDURE usp_AddUpdateAritcleType
@AritcleTypeCode nvarchar(30) = null,
@AritcleTypeDesc nvarchar(30) = null,
@UpdatedByUserID nvarchar(30) = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	IF Exists(SELECT 1 FROM tbl_AritcleType WHERE AritcleTypeCode=@AritcleTypeCode)
	BEGIN
		SET @RandNum = @AritcleTypeCode
		UPDATE tbl_AritcleType SET 
				AritcleTypeDesc = ISNULL(@AritcleTypeDesc,AritcleTypeDesc),
				UpdatedByUserID = ISNULL(@UpdatedByUserID,UpdatedByUserID),
				LastUpdatedDate = GETDATE()
		WHERE AritcleTypeCode = @AritcleTypeCode
	END
	ELSE
	BEGIN
		EXEC usp_GetRandNum @RandNum out
		SET @RandNum='Atp'+@RandNum
		INSERT INTO tbl_AritcleType(
			AritcleTypeCode,
			AritcleTypeDesc,
			UpdatedByUserID,
			LastUpdatedDate
		)VALUES(
			@RandNum,
			@AritcleTypeDesc,
			@UpdatedByUserID,
			GETDATE()
		)
	END
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT AritcleType.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT AritcleTypeCode = @RandNum,StatusCode='0',StatusDesc='Success'
	END
END