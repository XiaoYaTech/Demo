USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateReadRoleType')
BEGIN
	DROP PROCEDURE usp_AddUpdateReadRoleType
END
GO

CREATE PROCEDURE usp_AddUpdateReadRoleType
@ReadRoleTypeCode nvarchar(30) = null,
@ReadRoleTypeDesc nvarchar(30) = null,
@UpdatedByUserID nvarchar(30) = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	IF Exists(SELECT 1 FROM tbl_ReadRoleType WHERE ReadRoleTypeCode=@ReadRoleTypeCode)
	BEGIN
		SET @RandNum = @ReadRoleTypeCode
		UPDATE tbl_ReadRoleType SET 
				ReadRoleTypeDesc = ISNULL(@ReadRoleTypeDesc,ReadRoleTypeDesc),
				UpdatedByUserID = ISNULL(@UpdatedByUserID,UpdatedByUserID),
				LastUpdatedDate = GETDATE()
		WHERE ReadRoleTypeCode = @ReadRoleTypeCode
	END
	ELSE
	BEGIN
		EXEC usp_GetRandNum @RandNum out
		SET @RandNum='Rdr'+@RandNum
		INSERT INTO tbl_ReadRoleType(
			ReadRoleTypeCode,
			ReadRoleTypeDesc,
			UpdatedByUserID,
			LastUpdatedDate
		)VALUES(
			@RandNum,
			@ReadRoleTypeDesc,
			@UpdatedByUserID,
			GETDATE()
		)
	END
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT ReadRoleType.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT AritcleTypeCode = @RandNum,StatusCode='0',StatusDesc='Success'
	END
END