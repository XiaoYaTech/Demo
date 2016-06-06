USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateRole')
BEGIN
	DROP PROCEDURE usp_AddUpdateRole
END
GO

CREATE PROCEDURE usp_AddUpdateRole
@RoleID int = null,
@RoleName nvarchar(30) = null,
@RoleDesc nvarchar(100) = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	IF Exists(SELECT 1 FROM tbl_Role WHERE RoleID=@RoleID)
	BEGIN
		UPDATE tbl_Role 
			SET RoleName = ISNULL(@RoleName,RoleName),
				RoleDesc = ISNULL(@RoleDesc,RoleDesc),
				LastUpdatedDate = GETDATE()
		WHERE RoleID = @RoleID
	END
	ELSE
	BEGIN
		INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate)values(@RoleName,@RoleDesc,GETDATE())
	END
	IF(@@ERROR=0)
		SELECT StatusCode='0',StatusDesc='Success'
END
GO
