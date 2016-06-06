USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetReadRoleType')
BEGIN
	DROP PROCEDURE usp_GetReadRoleType
END
GO


CREATE PROCEDURE usp_GetReadRoleType
@ReadRoleTypeCode nvarchar(30)=null
AS
BEGIN
	IF @ReadRoleTypeCode IS NOT NULL
	BEGIN
		SELECT * FROM tbl_ReadRoleType WHERE ReadRoleTypeCode= @ReadRoleTypeCode;
	END
	ELSE
	BEGIN
		SELECT * FROM tbl_ReadRoleType;
	END
END
GO
