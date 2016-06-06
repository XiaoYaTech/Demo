USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetProductCategory')
BEGIN
	DROP PROCEDURE usp_GetProductCategory
END
GO

CREATE PROCEDURE usp_GetProductCategory
@CategoryID nvarchar(30)=null
AS
BEGIN
	SELECT * FROM tbl_ProductCategory WHERE CategoryID=ISNULL(@CategoryID,CategoryID)
END