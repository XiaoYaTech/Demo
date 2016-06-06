USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetProvince')
BEGIN
	DROP PROCEDURE usp_GetProvince
END
GO

CREATE PROCEDURE usp_GetProvince
@ProvinceName nvarchar(100) = null,
@ProvinceID nvarchar(8) = null
AS
BEGIN
	SELECT * FROM tbl_Province WHERE ProvinceName=ISNULL(@ProvinceName,ProvinceName) AND ProvinceID = ISNULL(@ProvinceID,ProvinceID)
END
GO
