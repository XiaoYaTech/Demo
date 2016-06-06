USE [MoyeBuyCom]
GO

DELETE tbl_LayoutMenu
GO

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'首页','http://www.moyebuy.com',1,'_self','NavHome','NavItem0','首页')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'标题1','http://www.moyebuy.com',2,'_blank','NavItem','NavItem1','标题1')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'标题2','http://www.moyebuy.com',3,'_blank','NavItem','NavItem2','标题2')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'标题3','http://www.moyebuy.com',4,'_blank','NavItem','NavItem3','标题3')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'标题4','http://www.moyebuy.com',5,'_blank','NavItem','NavItem4','标题4')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'标题5','http://www.moyebuy.com',6,'_blank','NavItem','NavItem5','标题5')
					
					
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('CategoryAll',0,'全部商品分类','http://www.moyebuy.com',1,'_self','CategorysTitle','CategorysTitle','全部商品分类')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('Category',0,'食品、饮料、酒水','http://www.moyebuy.com',2,'_self',null,'Categorys1','食品、饮料、酒水')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('Category',0,'厨房用品、个人护理','http://www.moyebuy.com',3,'_self',null,'Categorys2','厨房用品、个人护理')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('Category',0,'礼品箱包3','http://www.moyebuy.com',4,'_self',null,'Categorys3','礼品箱包')

				
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',0,'礼品','http://www.moyebuy.com',0,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','礼品')
				
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',0,'海鲜','http://www.moyebuy.com',1,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','海鲜')
		
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',1,'菜单管理','http://www.moyebuy.com',0,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','菜单管理')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',1,'产品管理','http://www.moyebuy.com',1,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','产品管理')
				
		
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'礼品1','http://www.moyebuy.com',1,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','礼品1')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'礼品2','http://www.moyebuy.com',2,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','礼品2')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'礼品礼品礼品3','http://www.moyebuy.com',3,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','礼品3')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'礼品4','http://www.moyebuy.com',4,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','礼品4')


INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'添加菜单','http://www.moyebuy.com',1,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','添加菜单')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'删除菜单','http://www.moyebuy.com',2,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','删除菜单')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'更新菜单','http://www.moyebuy.com',3,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','更新菜单')


INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'添加产品','http://www.moyebuy.com',1,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','添加产品')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'删除产品','http://www.moyebuy.com',2,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','删除产品')
					
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'更新产品','http://www.moyebuy.com',3,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','更新产品')






--MainSubCategory
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('8','11',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('8','16',GETDATE())

insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('14','22',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('14','23',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('14','24',GETDATE())

insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('13','19',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('13','20',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('13','21',GETDATE())

--------subCategory
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('11','12',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('11','13',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('11','14',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('11','15',GETDATE())

insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('16','12',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('16','13',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('16','14',GETDATE())
insert into tbl_LayoutSubMenuMapping(MenuID,SubMenuId,LastUpdatedDate) values('16','15',GETDATE())



