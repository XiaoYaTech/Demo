USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetReplyToArticle')
BEGIN
	DROP PROCEDURE usp_GetReplyToArticle
END
GO

CREATE PROCEDURE usp_GetReplyToArticle 
@ArticleID nvarchar(30) = null,
@ReplyID nvarchar(30) = null
AS
BEGIN
	IF @ReplyID IS NOT NULL
	BEGIN
		SELECT * FROM tbl_ReplyToArticle WHERE ReplyID = @ReplyID
	END
	ELSE
	BEGIN
		SELECT * FROM tbl_ReplyToArticle WHERE ArticleID = @ArticleID
	END
END