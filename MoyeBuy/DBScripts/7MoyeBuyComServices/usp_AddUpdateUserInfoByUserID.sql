USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateUserInfoByUserID')
BEGIN
	DROP PROCEDURE usp_AddUpdateUserInfoByUserID
END
GO

CREATE PROCEDURE usp_AddUpdateUserInfoByUserID
@UserID nvarchar(30)=null,
@UserPhoneNo nvarchar(30)=null,
@MoyeBuyComUserName nvarchar(30)=null,
@MoyeBuyComEmail nvarchar(50)=null,
@MoyeBuyComPwdSalt nvarchar(30)=null,
@MoyeBuyComPwdHash nvarchar(100)=null,
@RoleID int = null,
@AddressID nvarchar(30) = null,
@IsEffective bit = null,
@IsNeedChangePwd bit = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	DECLARE @ErrorDesc nvarchar(1000)
	DECLARE @CalledUSPName nvarchar(50)
	DECLARE @ErrorUSPName nvarchar(50)
	IF Exists(SELECT 1 FROM tbl_MoyeBuyComUser WHERE MoyeBuyComUserID=@UserID)
	BEGIN
		SET @RandNum = @UserID
		BEGIN TRY
		BEGIN TRANSACTION
		
		UPDATE tbl_MoyeBuyComUser 
				SET UserPhoneNo = ISNULL(@UserPhoneNo,UserPhoneNo) ,
					MoyeBuyComUserName = ISNULL(@MoyeBuyComUserName,MoyeBuyComUserName),
					MoyeBuyComPwdSalt = ISNULL(@MoyeBuyComPwdSalt,MoyeBuyComPwdSalt),
					MoyeBuyComPwdHash = ISNULL(@MoyeBuyComPwdHash,MoyeBuyComPwdHash),
					AddressID = ISNULL(@AddressID,AddressID),
					IsEffective = ISNULL(@IsEffective,IsEffective),
					IsNeedChangePwd = ISNULL(IsNeedChangePwd,@IsNeedChangePwd)
				WHERE MoyeBuyComUserID = @UserID;
		UPDATE tbl_UserRole SET RoleID = ISNULL(@RoleID,RoleID) WHERE MoyeBuyComUserID = @UserID

		COMMIT TRANSACTION
		IF(@@TRANCOUNT=0)
		BEGIN
			SELECT MoyeBuyComUserID=@RandNum,StatusCode='0',StatusDesc='Success'
			return 0
		END
		
		END TRY
		BEGIN CATCH
			IF(@@TRANCOUNT>0)
			BEGIN
				ROLLBACK TRANSACTION

				SET @ErrorDesc='Error Stored Procedure: ' + ISNULL(ERROR_PROCEDURE(),OBJECT_NAME(@@PROCID)) + ', Line Number: ' + CONVERT(nvarchar(20),ERROR_LINE()) + ', Message: ' + ERROR_MESSAGE()
				SET @CalledUSPName=OBJECT_NAME(@@PROCID)
				SET @ErrorUSPName=ISNULL(ERROR_PROCEDURE(),OBJECT_NAME(@@PROCID))
				SELECT StatusCode=9999, StatusDesc=@ErrorDesc
			END
		END CATCH
	END
	ELSE
	BEGIN
	BEGIN TRY
	BEGIN TRANSACTION
		EXEC usp_GetRandNum @RandNum out
		set @RandNum='UID'+@RandNum
		INSERT INTO tbl_MoyeBuyComUser(
							MoyeBuyComUserID,
							AddressID,
							MoyeBuyComUserName,
							UserPhoneNo,
							MoyeBuyComEmail,
							MoyeBuyComPwdSalt,
							MoyeBuyComPwdHash,
							LastUpdatedDate,
							IsEffective,
							IsNeedChangePwd)
					VALUES(@RandNum,
							@AddressID,
							@MoyeBuyComUserName,
							@UserPhoneNo,
							@MoyeBuyComEmail,
							@MoyeBuyComPwdSalt,
							@MoyeBuyComPwdHash,
							GETDATE(),
							1,
							0)
		
		IF @RoleID IS NULL
		BEGIN
			SELECT @RoleID = RoleID FROM tbl_Role WHERE RoleName='USER'
		END
		INSERT INTO tbl_UserRole (MoyeBuyComUserID,RoleID,LastUpdatedDate) VALUES(@RandNum,@RoleID,GETDATE())
	COMMIT TRANSACTION
	BEGIN
	IF(@@TRANCOUNT=0)
		SELECT MoyeBuyComUserID=@RandNum,StatusCode='0',StatusDesc='Success'
		return 0
	END
	
	END TRY
	BEGIN CATCH
		IF(@@TRANCOUNT>0)
		BEGIN
			ROLLBACK TRANSACTION
			
			SET @ErrorDesc='Error Stored Procedure: ' + ISNULL(ERROR_PROCEDURE(),OBJECT_NAME(@@PROCID)) + ', Line Number: ' + CONVERT(nvarchar(20),ERROR_LINE()) + ', Message: ' + ERROR_MESSAGE()
			SET @CalledUSPName=OBJECT_NAME(@@PROCID)
			SET @ErrorUSPName=ISNULL(ERROR_PROCEDURE(),OBJECT_NAME(@@PROCID))
			SELECT StatusCode=9999, StatusDesc=@ErrorDesc
		END
	END CATCH
	END
END
GO
