USE [MoyeBuyComServices]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_District')
BEGIN
	DROP PROCEDURE usp_District
END
GO

CREATE PROCEDURE usp_District
@DistrictID nvarchar(8) = null,
@CityID nvarchar(8) = null,
@DistrictName nvarchar(100) = null
AS
BEGIN
	SELECT * FROM tbl_District WHERE DistrictID=ISNULL(@DistrictID,DistrictID) 
							AND CityID = ISNULL(@CityID,CityID)
							AND DistrictName = ISNULL(@DistrictName,DistrictName)
END
GO
