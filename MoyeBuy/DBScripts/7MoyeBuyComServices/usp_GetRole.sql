USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetRole')
BEGIN
	DROP PROCEDURE usp_GetRole
END
GO

CREATE PROCEDURE usp_GetRole
AS
BEGIN
	SELECT * FROM tbl_Role
END
GO
