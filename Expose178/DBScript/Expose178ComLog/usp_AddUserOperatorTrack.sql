USE [Expose178ComLog]
Go

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUserOperatorTrack')
BEGIN
	DROP PROCEDURE usp_AddUserOperatorTrack
END
GO

CREATE PROCEDURE usp_AddUserOperatorTrack
@ParentUserOperatorTrackID nvarchar(30)=null,
@UserID nvarchar(30),
@FromURL nvarchar(500)=null,
@PageName nvarchar(30)=null
AS
BEGIN
	DECLARE @RANDNUM NVARCHAR(30)
	EXEC [Expose178Com].dbo.usp_GetRandNum @RANDNUM out
	SET @RANDNUM = 'Trck'+@RANDNUM
	INSERT INTO tbl_UserOperatorTrack(UserOperatorTrackID,ParentUserOperatorTrackID,UserID,FromURL,PageName,LastUpdatedDate)
	VALUES(@RANDNUM,@ParentUserOperatorTrackID,@UserID,@FromURL,@PageName,GETDATE())
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_UserOperatorTrack.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT UserOperatorTrackID=@RANDNUM,StatusCode='0',StatusDesc='Success'
		return 0
	END
END