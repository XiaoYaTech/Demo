USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateArticleAdditionalByAdditionalID')
BEGIN
	DROP PROCEDURE usp_AddUpdateArticleAdditionalByAdditionalID
END
GO

CREATE PROCEDURE usp_AddUpdateArticleAdditionalByAdditionalID
@AdditionalID nvarchar(30) = null,
@ArticleID nvarchar(30) = null,
@ReadNum int = null,
@ReplyNum int = null,
@UpdatedByUserID nvarchar(30) = null
AS
BEGIN
	DECLARE @RandNum nvarchar(30)
	IF Exists(SELECT 1 FROM tbl_ArticleAdditional WHERE AdditionalID=@AdditionalID)
	BEGIN
		SET @RandNum = @AdditionalID
		UPDATE tbl_ArticleAdditional SET 
				ArticleID = ISNULL(@ArticleID,ArticleID),
				ReadNum = ISNULL(@ReadNum,ReadNum),
				ReplyNum = ISNULL(@ReplyNum,ReplyNum),
				UpdatedByUserID = ISNULL(@UpdatedByUserID,UpdatedByUserID),
				LastUpdatedDate = GETDATE()
		WHERE AdditionalID = @AdditionalID
	END
	ELSE
	BEGIN
		EXEC usp_GetRandNum @RandNum out
		SET @RandNum='Add'+@RandNum
		INSERT INTO tbl_ArticleAdditional(
			AdditionalID,
			ArticleID,
			ReadNum,
			ReplyNum,
			UpdatedByUserID,
			LastUpdatedDate
		)VALUES(
			@RandNum,
			@ArticleID,
			@ReadNum,
			@ReplyNum,
			@UpdatedByUserID,
			GETDATE()
		)
	END
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT ArticleAdditional.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT AdditionalID = @RandNum,StatusCode='0',StatusDesc='Success'
	END
END