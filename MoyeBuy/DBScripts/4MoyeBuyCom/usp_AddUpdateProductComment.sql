USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateProductComment')
BEGIN
	DROP PROCEDURE usp_AddUpdateProductComment
END
GO

CREATE PROCEDURE usp_AddUpdateProductComment
@CommentID nvarchar(30)=null,
@ParentCommentID nvarchar(30)=null,
@UID nvarchar(30),
@ProductID nvarchar(30),
@CommentCatgoryID int,
@CommentDesc nvarchar(1000),
@CommentState nvarchar(10),
@CommentAttitude nvarchar(10),
@IsAgree bit
AS
BEGIN
DECLARE @RandNum NVARCHAR(30)
	IF @CommentID IS NOT NULL
	BEGIN
		SET @RandNum = @CommentID
		UPDATE tbl_Comment SET 
			ParentCommentID=@ParentCommentID,
			UserID = @UID,
			ProductID = @ProductID,
			CommentCatgoryID=@CommentCatgoryID,
			CommentDesc = @CommentDesc,
			CommentState = @CommentState,
			CommentAttitude=@CommentAttitude,
			IsAgree=@IsAgree,
			LastUpdatedDate = GETDATE()
			WHERE CommentID = @CommentID
	END	
	ELSE
	BEGIN
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	SET @RandNum='CMTS'+@RandNum
		INSERT INTO tbl_Comment(
		CommentID,
		ParentCommentID,
		UserID,
		ProductID,
		CommentCatgoryID,
		CommentDesc,
		CommentState,
		CommentAttitude,
		IsAgree,
		LastUpdatedDate)
		VALUES(
		@RandNum,
		NULL,
		@UID,
		@ProductID,
		@CommentCatgoryID,
		@CommentDesc,
		@CommentState,
		@CommentAttitude,
		@IsAgree,
		GETDATE())
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_Comment.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT CommentID= @RandNum,StatusCode='0',StatusDesc='Success'
	END
END
