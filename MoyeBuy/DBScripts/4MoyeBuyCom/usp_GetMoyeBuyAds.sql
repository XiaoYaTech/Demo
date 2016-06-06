USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetMoyeBuyAds')
BEGIN
	DROP PROCEDURE usp_GetMoyeBuyAds
END
GO

CREATE PROCEDURE usp_GetMoyeBuyAds
AS
BEGIN
	SELECT * FROM tbl_Advertisement
END
