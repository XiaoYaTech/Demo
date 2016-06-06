USE [Expose178Com]
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
	IF @UserID IS NULL
	BEGIN
		SELECT * FROM tbl_User WHERE UserEmail= @Email;
	END
	ELSE
	BEGIN
		Select * FROM tbl_User WHERE UserID=@UserID;
	END
END
GO
