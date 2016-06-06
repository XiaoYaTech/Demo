USE [MoyeBuyComLog]
Go

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddSystemLog')
BEGIN
	DROP PROCEDURE usp_AddSystemLog
END
GO

CREATE PROCEDURE usp_AddSystemLog
@SystemLogMsg nvarchar(max),
@UpdatedByUserID nvarchar(30),
@SystemLogPosition nvarchar(500),
@LastUpdatedDate datetime= null
AS
BEGIN
	DECLARE @RANDNUM NVARCHAR(30)
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RANDNUM out
	SET @RANDNUM='Log'+@RANDNUM
	INSERT INTO tbl_SystemLog(SystemLogID,SystemLogMsg,SystemLogPosition,UpdatedByUserID,LastUpdatedDate)
	VALUES(@RANDNUM,@SystemLogMsg,@SystemLogPosition,@UpdatedByUserID,ISNULL(@LastUpdatedDate,GETDATE()))
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_SystemLog.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT SystemLogID=@RANDNUM,StatusCode='0',StatusDesc='Success'
		return 0
	END
	
END
GO

