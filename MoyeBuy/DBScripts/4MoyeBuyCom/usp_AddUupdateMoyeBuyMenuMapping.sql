USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateSubMenuMapping')
BEGIN
	DROP PROCEDURE usp_AddUpdateSubMenuMapping
END
GO

CREATE PROCEDURE usp_AddUpdateSubMenuMapping
@SubMenuMappingID int = null,
@MenuID int = null,
@SubMenuID int = null
AS
BEGIN
	IF EXISTS(SELECT 1 FROM tbl_LayoutSubMenuMapping WHERE SubMenuMappingID = @SubMenuMappingID)
	BEGIN
		UPDATE tbl_LayoutSubMenuMapping SET 
			MenuID = ISNULL(@MenuID,MenuID),
			SubMenuId = ISNULL(@SubMenuId,SubMenuId),
			LastUpdatedDate = GETDATE()
			WHERE SubMenuMappingID = @SubMenuMappingID
	END
	ELSE
	BEGIN
		INSERT INTO tbl_LayoutSubMenuMapping (
			MenuID,
			SubMenuId,
			LastUpdatedDate
		)
		VALUES
		(
			@MenuID,
			@SubMenuId,
			GETDATE()
		)
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
GO