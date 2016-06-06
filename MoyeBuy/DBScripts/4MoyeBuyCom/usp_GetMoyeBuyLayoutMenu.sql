USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetMoyeBuyLayoutMenu')
BEGIN
	DROP PROCEDURE usp_GetMoyeBuyLayoutMenu
END
GO
CREATE PROCEDURE usp_GetMoyeBuyLayoutMenu
@MenuType nvarchar(20) = 'ALL',
@IsAdminMenu bit = 0
AS
BEGIN
	IF(@MenuType='ALL')
	BEGIN
		SELECT menu.*,map.SubMenuID,map.SubMenuMappingID FROM tbl_LayoutMenu menu LEFT JOIN tbl_LayoutSubMenuMapping map 
			ON menu.MenuID = map.MenuID WHERE menu.IsAdminMenu=@IsAdminMenu
	END
	ELSE
	BEGIN
		SELECT menu.*,map.SubMenuID,map.SubMenuMappingID FROM tbl_LayoutMenu menu LEFT JOIN tbl_LayoutSubMenuMapping map 
			ON menu.MenuID = map.MenuID
		WHERE menu.MenuType = ISNULL(@MenuType,menu.MenuType) AND menu.IsAdminMenu=@IsAdminMenu
	END
	
END

