USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateSubMenuMapping')
BEGIN
	DROP PROCEDURE usp_AddUpdateSubMenuMapping
END
GO

CREATE PROCEDURE usp_AddUpdateSubMenuMapping
@SubMenuMappingIDs nvarchar(max) = null,
@MenuID int = null,
@SubMenuIDs nvarchar(max) = null
AS
BEGIN
	IF(@SubMenuIDs IS NOT NULL)
	BEGIN
		DECLARE @iCount_MenuID int
		DECLARE @ilength_MenuID int
		
		DECLARE @iCount_SubMenuMappingID int
		DECLARE @ilength_SubMenuMappingID int
		
		SET @iCount_MenuID = 1
		WHILE(@iCount_MenuID <> 0)
		BEGIN
			SET @iCount_MenuID = PATINDEX('%|||%',@SubMenuIDs)
			SET @ilength_MenuID = CASE @iCount_MenuID WHEN 0 THEN DATALENGTH(@SubMenuIDs) ELSE @iCount_MenuID-1 END
			DECLARE @SubMenuID int
			SET @SubMenuID = CONVERT(int, SUBSTRING(@SubMenuIDs,1,@ilength_MenuID))
			
			SET @iCount_SubMenuMappingID = PATINDEX('%|||%',@SubMenuMappingIDs)
			SET @ilength_SubMenuMappingID = CASE @iCount_SubMenuMappingID WHEN 0 THEN DATALENGTH(@SubMenuMappingIDs) ELSE @iCount_SubMenuMappingID-1 END
			DECLARE @SubMenuMappingID int
			SET @SubMenuMappingID = CONVERT(int, SUBSTRING(@SubMenuMappingIDs,1,@ilength_SubMenuMappingID))
			
			IF EXISTS(SELECT 1 FROM tbl_LayoutSubMenuMapping WHERE SubMenuMappingID = @SubMenuMappingID)
			BEGIN
				UPDATE tbl_LayoutSubMenuMapping SET 
					MenuID = ISNULL(@MenuID,MenuID),
					SubMenuID = ISNULL(@SubMenuID,SubMenuID),
					LastUpdatedDate = GETDATE()
					WHERE SubMenuMappingID = @SubMenuMappingID
			END
			ELSE
			BEGIN
				IF(@iCount_MenuID=1)
				BEGIN
					DELETE FROM tbl_LayoutSubMenuMapping WHERE MenuID=@MenuID
				END
				INSERT INTO tbl_LayoutSubMenuMapping (
					MenuID,
					SubMenuID,
					LastUpdatedDate
				)
				VALUES
				(
					@MenuID,
					@SubMenuID,
					GETDATE()
				)
			END
			
			SET @SubMenuIDs = SUBSTRING(@SubMenuIDs,@iCount_MenuID+3,DATALENGTH(@SubMenuIDs))
			SET @SubMenuMappingIDs = SUBSTRING(@SubMenuMappingIDs,@iCount_SubMenuMappingID+3,DATALENGTH(@SubMenuMappingIDs))
		END
		IF(@@ERROR <> 0)
		BEGIN
			SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_LayoutSubMenuMapping' 
		END
		ELSE
		BEGIN
		IF(@@ERROR=0)
			SELECT StatusCode='0',StatusDesc='Success'
		END
	END

END
GO

