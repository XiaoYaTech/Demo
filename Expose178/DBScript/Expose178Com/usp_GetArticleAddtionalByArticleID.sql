USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetArticleAddtionalByArticleID')
BEGIN
	DROP PROCEDURE usp_GetArticleAddtionalByArticleID
END
GO

CREATE PROCEDURE usp_GetArticleAddtionalByArticleID 
@ArticleID nvarchar(30)
AS
BEGIN
	SELECT * 
	FROM tbl_ArticleAdditional
	WHERE ArticleID = @ArticleID
END
 