USE [Expose178ComLog]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetUserTrackByUID')
BEGIN
	DROP PROCEDURE usp_GetUserTrackByUID
END
GO

CREATE PROCEDURE usp_GetUserTrackByUID
@UID nvarchar(30)=null
AS
BEGIN
	IF @UID IS NOT NULL
	BEGIN
		SELECT * FROM tbl_UserOperatorTrack WHERE UserID= @UID
	END
	ELSE
	BEGIN
		SELECT * FROM tbl_UserOperatorTrack
	END
END