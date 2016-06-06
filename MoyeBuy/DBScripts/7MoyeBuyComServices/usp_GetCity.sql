USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetCity')
BEGIN
	DROP PROCEDURE usp_GetCity
END
GO

CREATE PROCEDURE usp_GetCity
@CityName nvarchar(100) = null,
@CityID nvarchar(8) = null,
@ProvinceID nvarchar(8) = null,
@ZipCode nvarchar(100) = null
AS
BEGIN
	SELECT * FROM tbl_City WHERE CityName=ISNULL(@CityName,CityName) 
							AND ProvinceID = ISNULL(@ProvinceID,ProvinceID)
							AND CityID = ISNULL(@CityID,CityID)
							AND ZipCode = ISNULL(@ZipCode,ZipCode)
END
GO
