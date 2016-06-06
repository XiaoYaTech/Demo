USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetArticleByArticleID')
BEGIN
	DROP PROCEDURE usp_GetArticleByArticleID
END
GO

CREATE PROCEDURE usp_GetArticleByArticleID 
@ArticleID nvarchar(30)
AS
BEGIN
	SELECT * 
	FROM tbl_Article
	WHERE ArticleID = @ArticleID
END