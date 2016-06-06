USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetProductCommentByProductID')
BEGIN
	DROP PROCEDURE usp_GetProductCommentByProductID
END
GO

CREATE PROCEDURE usp_GetProductCommentByProductID
@ProductID nvarchar(30)
AS
BEGIN
	SELECT com.*,u.MoyeBuyComUserName FROM tbl_Comment com INNER JOIN MoyeBuyComServices.dbo.tbl_MoyeBuyComUser u
	ON com.UserID = u.MoyeBuyComUserID
	WHERE com.ProductID = @ProductID
END