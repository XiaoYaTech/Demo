USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateArticleByArticleID')
BEGIN
	DROP PROCEDURE usp_AddUpdateArticleByArticleID
END
GO

CREATE PROCEDURE usp_AddUpdateArticleByArticleID
@ArticleID nvarchar(50)=null,
@ArticleTile nvarchar(300)=null,
@ArticleBody nvarchar(max)=null,
@BackgroundImgUrl nvarchar(300) = null,
@IsValidated bit = 1,
@IsDraft bit = 0,
@AritcleTypeCode nvarchar(30) = null,
@ReadRoleTypeCode nvarchar(30) = null,
@UpdatedByUserID nvarchar(30) = null
AS
BEGIN
DECLARE @RandNum nvarchar(30)
	IF Exists(SELECT 1 FROM tbl_Article WHERE ArticleID=@ArticleID)
	BEGIN
		SET @RandNum = @ArticleID
		UPDATE tbl_Article SET 
				ArticleTile = ISNULL(@ArticleTile,ArticleTile),
				ArticleBody = ISNULL(@ArticleBody,ArticleBody),
				BackgroundImgUrl = ISNULL(@BackgroundImgUrl,BackgroundImgUrl),
				IsValidated = ISNULL(@IsValidated,IsValidated),
				IsDraft = ISNULL(@IsDraft,IsDraft),
				AritcleTypeCode = ISNULL(@AritcleTypeCode,AritcleTypeCode),
				ReadRoleTypeCode = ISNULL(@ReadRoleTypeCode,ReadRoleTypeCode),
				UpdatedByUserID = ISNULL(@UpdatedByUserID,UpdatedByUserID),
				LastUpdatedDate = GETDATE()
		WHERE ArticleID = @ArticleID
	END
	ELSE
	BEGIN
		EXEC usp_GetRandNum @RandNum out
		SET @RandNum='Art'+@RandNum
		INSERT INTO tbl_Article(
			ArticleID,
			ArticleTile,
			ArticleBody,
			ArticleDate,
			BackgroundImgUrl,
			IsValidated,
			IsDraft,
			AritcleTypeCode,
			ReadRoleTypeCode,
			UpdatedByUserID,
			LastUpdatedDate
		)VALUES(
			@RandNum,
			@ArticleTile,
			@ArticleBody,
			GETDATE(),
			@BackgroundImgUrl,
			@IsValidated,
			@IsDraft,
			@AritcleTypeCode,
			@ReadRoleTypeCode,
			@UpdatedByUserID,
			GETDATE()
		)
		EXEC usp_AddUpdateArticleAdditionalByAdditionalID @ArticleID=@RandNum,@ReadNum=0,@ReplyNum=0,@UpdatedByUserID=@UpdatedByUserID
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT Article.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT ArticleID= @RandNum,StatusCode='0',StatusDesc='Success'
	END
END
GO

