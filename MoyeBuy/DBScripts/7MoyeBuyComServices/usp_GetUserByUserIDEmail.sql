USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetUserByUserIDEmail')
BEGIN
	DROP PROCEDURE usp_GetUserByUserIDEmail
END
GO

CREATE PROCEDURE usp_GetUserByUserIDEmail
@UserID nvarchar(30)=null,
@Email nvarchar(50)=null
AS
BEGIN
	IF @UserID IS NOT NULL
	BEGIN
		SELECT u.*,
				r.RoleID,
				r.RoleName,
				r.RoleDesc 
				FROM tbl_MoyeBuyComUser u LEFT JOIN tbl_UserRole ur
						  ON u.MoyeBuyComUserID = ur.MoyeBuyComUserID
						INNER JOIN tbl_Role r
							ON r.RoleID = ur.RoleID
				WHERE u.MoyeBuyComUserID= @UserID;
	END
	ELSE
	BEGIN
		SELECT u.*,
				r.RoleID,
				r.RoleName,
				r.RoleDesc 
				FROM tbl_MoyeBuyComUser u LEFT JOIN tbl_UserRole ur
						  ON u.MoyeBuyComUserID = ur.MoyeBuyComUserID
						INNER JOIN tbl_Role r
							ON r.RoleID = ur.RoleID
				WHERE u.MoyeBuyComEmail= ISNULL(@Email,u.MoyeBuyComEmail)
	END
END
GO