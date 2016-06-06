USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateMoyeBuyMenu')
BEGIN
	DROP PROCEDURE usp_AddUpdateMoyeBuyMenu
END
GO

CREATE PROCEDURE usp_AddUpdateMoyeBuyMenu
@MenuIDs nvarchar(max) = null,
@MenuTypes nvarchar(max) = null,
@IsAdminMenus nvarchar(max) = null,
@MenuNames nvarchar(max) = null,
@MenuURLs nvarchar(max) = null,
@MenuDisqs nvarchar(max) = null,
@MenuTargets nvarchar(max) = null,
@MenuClassNames nvarchar(max) = null,
@MenuControlIDs nvarchar(max) = null,
@MenuTitles nvarchar(max) = null
AS
BEGIN
	IF(@MenuNames IS NOT NULL)
	BEGIN
		DECLARE @NewMenuIDs nvarchar(max)
		SET @NewMenuIDs=''
		
		DECLARE @iCount_MenuID int
		DECLARE @ilength_MenuID int
		
		DECLARE @iCount_MenuType int
		DECLARE @ilength_MenuType int
		
		DECLARE @iCount_IsAdminMenu int
		DECLARE @ilength_IsAdminMenu int
		
		DECLARE @iCount_MenuName int
		DECLARE @ilength_MenuName int
		
		DECLARE @iCount_MenuURL int
		DECLARE @ilength_MenuURL int
		
		DECLARE @iCount_MenuDisq int
		DECLARE @ilength_MenuDisq int
		
		DECLARE @iCount_MenuTarget int
		DECLARE @ilength_MenuTarget int
		
		DECLARE @iCount_MenuClassName int
		DECLARE @ilength_MenuClassName int
		
		DECLARE @iCount_MenuControlID int
		DECLARE @ilength_MenuControlID int
		
		DECLARE @iCount_MenuTitle int
		DECLARE @ilength_MenuTitle int
		
		SET @iCount_MenuName = 1
		WHILE(@iCount_MenuName <> 0)
		BEGIN
			SET @iCount_MenuName = PATINDEX('%|||%',@MenuNames)
			SET @ilength_MenuName = CASE @iCount_MenuName WHEN 0 THEN DATALENGTH(@MenuNames) ELSE @iCount_MenuName-1 END
			DECLARE @MenuName nvarchar(100)
			SET @MenuName = CONVERT(nvarchar(100), SUBSTRING(@MenuNames,1,@ilength_MenuName))
			
			SET @iCount_MenuID = PATINDEX('%|||%',@MenuIDs)
			SET @ilength_MenuID = CASE @iCount_MenuID WHEN 0 THEN DATALENGTH(@MenuIDs) ELSE @iCount_MenuID-1 END
			DECLARE @MenuID int
			SET @MenuID = CONVERT(int, SUBSTRING(@MenuIDs,1,@ilength_MenuID))

			SET @iCount_MenuType = PATINDEX('%|||%',@MenuTypes)
			SET @ilength_MenuType = CASE @iCount_MenuType WHEN 0 THEN DATALENGTH(@MenuTypes) ELSE @iCount_MenuType-1 END
			DECLARE @MenuType nvarchar(100)
			SET @MenuType = CONVERT(nvarchar(100), SUBSTRING(@MenuTypes,1,@ilength_MenuType))

			SET @iCount_IsAdminMenu = PATINDEX('%|||%',@IsAdminMenus)
			SET @ilength_IsAdminMenu = CASE @iCount_IsAdminMenu WHEN 0 THEN DATALENGTH(@IsAdminMenus) ELSE @iCount_IsAdminMenu-1 END
			DECLARE @IsAdminMenu bit
			SET @IsAdminMenu = CONVERT(bit, SUBSTRING(@IsAdminMenus,1,@ilength_IsAdminMenu))
			
			SET @iCount_MenuURL = PATINDEX('%|||%',@MenuURLs)
			SET @ilength_MenuURL = CASE @iCount_MenuURL WHEN 0 THEN DATALENGTH(@MenuURLs) ELSE @iCount_MenuURL-1 END
			DECLARE @MenuURL nvarchar(600)
			SET @MenuURL = CONVERT(nvarchar(600), SUBSTRING(@MenuURLs,1,@ilength_MenuURL))
			
			SET @iCount_MenuDisq = PATINDEX('%|||%',@MenuDisqs)
			SET @ilength_MenuDisq = CASE @iCount_MenuDisq WHEN 0 THEN DATALENGTH(@MenuDisqs) ELSE @iCount_MenuDisq-1 END
			DECLARE @MenuDisq int
			SET @MenuDisq = CONVERT(int, SUBSTRING(@MenuDisqs,1,@ilength_MenuDisq))
			
			SET @iCount_MenuTarget = PATINDEX('%|||%',@MenuTargets)
			SET @ilength_MenuTarget = CASE @iCount_MenuTarget WHEN 0 THEN DATALENGTH(@MenuTargets) ELSE @iCount_MenuTarget-1 END
			DECLARE @MenuTarget nvarchar(20)
			SET @MenuTarget = CONVERT(nvarchar(20), SUBSTRING(@MenuTargets,1,@ilength_MenuTarget))
			
			SET @iCount_MenuClassName = PATINDEX('%|||%',@MenuClassNames)
			SET @ilength_MenuClassName = CASE @iCount_MenuClassName WHEN 0 THEN DATALENGTH(@MenuClassNames) ELSE @iCount_MenuClassName-1 END
			DECLARE @MenuClassName nvarchar(100)
			SET @MenuClassName = CONVERT(nvarchar(100), SUBSTRING(@MenuClassNames,1,@ilength_MenuClassName))

			SET @iCount_MenuControlID = PATINDEX('%|||%',@MenuControlIDs)
			SET @ilength_MenuControlID = CASE @iCount_MenuControlID WHEN 0 THEN DATALENGTH(@MenuControlIDs) ELSE @iCount_MenuControlID-1 END
			DECLARE @MenuControlID nvarchar(100)
			SET @MenuControlID = CONVERT(nvarchar(100), SUBSTRING(@MenuControlIDs,1,@ilength_MenuControlID))
	
			SET @iCount_MenuTitle = PATINDEX('%|||%',@MenuTitles)
			SET @ilength_MenuTitle = CASE @iCount_MenuTitle WHEN 0 THEN DATALENGTH(@MenuTitles) ELSE @iCount_MenuTitle-1 END
			DECLARE @MenuTitle nvarchar(100)
			SET @MenuTitle = CONVERT(nvarchar(100), SUBSTRING(@MenuTitles,1,@ilength_MenuTitle))

			IF EXISTS(SELECT 1 FROM tbl_LayoutMenu WHERE MenuID = @MenuID)
			BEGIN
				UPDATE tbl_LayoutMenu SET 
					MenuType = ISNULL(@MenuType,MenuType),
					IsAdminMenu = ISNULL(@IsAdminMenu,IsAdminMenu),
					MenuName = ISNULL(@MenuName,MenuName),
					MenuURL = ISNULL(@MenuURL,MenuURL),
					MenuDisq = ISNULL(@MenuDisq,MenuDisq),
					MenuTarget = ISNULL(@MenuTarget,MenuTarget),
					MenuClassName = ISNULL(@MenuClassName,MenuClassName),
					MenuControlID = ISNULL(@MenuControlID,MenuControlID),
					MenuTitle = ISNULL(@MenuTitle,MenuTitle)
					WHERE MenuID = @MenuID
					
					SET @NewMenuIDs = @NewMenuIDs+CONVERT(NVARCHAR,@MenuID)+'|||'
			END
			ELSE
			BEGIN
				INSERT INTO tbl_LayoutMenu(
					MenuType,
					IsAdminMenu,
					MenuName,
					MenuURL,
					MenuDisq,
					MenuTarget,
					MenuClassName,
					MenuControlID,
					MenuTitle,
					LastUpdatedDate
				)
				VALUES
				(
					@MenuType,
					@IsAdminMenu,
					@MenuName,
					@MenuURL,
					@MenuDisq,
					@MenuTarget,
					@MenuClassName,
					@MenuControlID,
					@MenuTitle,
					GETDATE()
				)
				SET @NewMenuIDs = @NewMenuIDs+CONVERT(NVARCHAR,@@IDENTITY)+'|||'
			END
			IF(@@ERROR <> 0)
			BEGIN
				SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_LayoutMenu' 
			END

			SET @MenuNames = SUBSTRING(@MenuNames,@iCount_MenuName+3,DATALENGTH(@MenuNames))
			SET @MenuIDs = SUBSTRING(@MenuIDs,@iCount_MenuID+3,DATALENGTH(@MenuIDs))
			SET @MenuTypes = SUBSTRING(@MenuTypes,@iCount_MenuType+3,DATALENGTH(@MenuTypes))
			SET @IsAdminMenus = SUBSTRING(@IsAdminMenus,@iCount_IsAdminMenu+3,DATALENGTH(@IsAdminMenus))
			SET @MenuURLs = SUBSTRING(@MenuURLs,@iCount_MenuURL+3,DATALENGTH(@MenuURLs))
			SET @MenuDisqs = SUBSTRING(@MenuDisqs,@iCount_MenuDisq+3,DATALENGTH(@MenuDisqs))
			SET @MenuTargets = SUBSTRING(@MenuTargets,@iCount_MenuTarget+3,DATALENGTH(@MenuTargets))
			SET @MenuClassNames = SUBSTRING(@MenuClassNames,@iCount_MenuClassName+3,DATALENGTH(@MenuClassNames))
			SET @MenuControlIDs = SUBSTRING(@MenuControlIDs,@iCount_MenuControlID+3,DATALENGTH(@MenuControlIDs))
			SET @MenuTitles = SUBSTRING(@MenuTitles,@iCount_MenuTitle+3,DATALENGTH(@MenuTitles))
		END
		SET @NewMenuIDs = SUBSTRING(@NewMenuIDs,1,len(@NewMenuIDs)-3)
		IF(@@ERROR=0)
			SELECT StatusCode='0',StatusDesc='Success',NewMenuIDs = @NewMenuIDs
	END
END
GO