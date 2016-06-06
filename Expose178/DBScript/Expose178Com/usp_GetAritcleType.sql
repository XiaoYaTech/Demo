USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetAritcleType')
BEGIN
	DROP PROCEDURE usp_GetAritcleType
END
GO

CREATE PROCEDURE usp_GetAritcleType
@AritcleTypeCode nvarchar(30)=null,
@UserID nvarchar(30)=null
AS
BEGIN
	IF @AritcleTypeCode IS NOT NULL
	BEGIN
		SELECT * FROM tbl_AritcleType WHERE AritcleTypeCode= @AritcleTypeCode;
	END
	ELSE
	BEGIN
		IF @UserID IS NOT NULL
		BEGIN
			SELECT * FROM tbl_AritcleType WHERE UpdatedByUserID=@UserID;
		END
		ELSE
		BEGIN
			SELECT * FROM tbl_AritcleType;
		END
	END
END
GO
