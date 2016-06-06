USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateUserInfoByUserID')
BEGIN
	DROP PROCEDURE usp_AddUpdateUserInfoByUserID
END
GO

CREATE PROCEDURE usp_AddUpdateUserInfoByUserID
@UserID nvarchar(30)=null,
@UserName nvarchar(30)=null,
@UserEmail nvarchar(50)=null,
@PwdSalt nvarchar(30)=null,
@PwdHash nvarchar(100)=null,
@IsEffective bit = null,
@IsNeedChangePwd bit = null,
@IsTempUser bit = 0,
@IP nvarchar(50) = '127.0.0.1',
@Browser nvarchar(100) = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	IF @IsTempUser = 0
	BEGIN
		IF Exists(SELECT 1 FROM tbl_User WHERE UserID=@UserID)
		BEGIN
			SET @RandNum = @UserID
			UPDATE tbl_User SET UserName = ISNULL(@UserName,UserName),
								PwdSalt = ISNULL(@PwdSalt,PwdSalt),
								PwdHash = ISNULL(@PwdHash,PwdHash),
								IsNeedChangePwd = ISNULL(@IsNeedChangePwd,IsNeedChangePwd),
								IsEffective=ISNULL(@IsEffective,IsEffective)
			WHERE UserID = @UserID;
		END
		ELSE
		BEGIN
			EXEC usp_GetRandNum @RandNum out
			set @RandNum='UID'+@RandNum
			INSERT INTO tbl_User(UserID,
								 UserName,
								 UserEmail,
								 PwdSalt,
								 PwdHash,
								 IsEffective,
								 IsNeedChangePwd,
								 LastUpdatedDate)
								VALUES(@RandNum,
								@UserName,
								@UserEmail,
								@PwdSalt,
								@PwdHash,
								@IsEffective,
								@IsNeedChangePwd,
								GETDATE())
		END
	END
	ELSE
	BEGIN
		EXEC usp_GetRandNum @RandNum out
			set @RandNum='TMP'+@RandNum
			INSERT INTO tbl_TempUser(UserID,
								 IP,
								 Browser,
								 LastUpdatedDate)
								VALUES(@RandNum,
								@IP,
								@Browser,
								GETDATE())
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT User.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT UserID=@RandNum,StatusCode='0',StatusDesc='Success'
		return 0
	END
	
END
GO
