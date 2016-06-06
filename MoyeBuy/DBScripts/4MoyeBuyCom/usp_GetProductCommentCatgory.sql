USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetProductCommentCatgory')
BEGIN
	DROP PROCEDURE usp_GetProductCommentCatgory
END
GO

CREATE PROCEDURE usp_GetProductCommentCatgory
@CommentCatgoryID nvarchar(30)=null
AS
BEGIN
	SELECT * FROM tbl_CommentCatgory WHERE CommentCatgoryID=ISNULL(@CommentCatgoryID,CommentCatgoryID)
END