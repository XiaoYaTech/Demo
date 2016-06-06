USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateReplyToArticleByReplyID')
BEGIN
	DROP PROCEDURE usp_AddUpdateReplyToArticleByReplyID
END
GO

CREATE PROCEDURE usp_AddUpdateReplyToArticleByReplyID
@ReplyID nvarchar(30) = null,
@ArticleID nvarchar(30) = null,
@ReplyBody nvarchar(1000) = null,
@IsValidated bit = 1,
@UpdatedByUserID nvarchar(30) = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	IF Exists(SELECT 1 FROM tbl_ReplyToArticle WHERE ReplyID=@ReplyID)
	BEGIN
		SET @RandNum = @ReplyID
		UPDATE tbl_ReplyToArticle SET 
				ArticleID = ISNULL(@ArticleID,ArticleID),
				ReplyBody = ISNULL(@ReplyBody,ReplyBody),
				IsValidated = ISNULL(@IsValidated,IsValidated),
				UpdatedByUserID = ISNULL(@UpdatedByUserID,UpdatedByUserID),
				LastUpdatedDate = GETDATE()
		WHERE ReplyID = @ReplyID
	END
	ELSE
	BEGIN
		EXEC usp_GetRandNum @RandNum out
		SET @RandNum='Rpl'+@RandNum
		INSERT INTO tbl_ReplyToArticle(
			ReplyID,
			ArticleID,
			ReplyBody,
			IsValidated,
			UpdatedByUserID,
			LastUpdatedDate
		)VALUES(
			@RandNum,
			@ArticleID,
			@ReplyBody,
			@IsValidated,
			@UpdatedByUserID,
			GETDATE()
		)
	END
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT ReplyToArticle.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT ReplyID = @RandNum,StatusCode='0',StatusDesc='Success'
	END
	
END