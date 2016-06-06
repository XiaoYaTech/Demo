USE [MoyeBuyCom]
GO

DELETE tbl_LayoutMenu
GO

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'��ҳ','http://www.moyebuy.com',1,'_self','NavHome','NavItem0','��ҳ')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'����1','http://www.moyebuy.com',2,'_blank','NavItem','NavItem1','����1')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'����2','http://www.moyebuy.com',3,'_blank','NavItem','NavItem2','����2')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'����3','http://www.moyebuy.com',4,'_blank','NavItem','NavItem3','����3')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'����4','http://www.moyebuy.com',5,'_blank','NavItem','NavItem4','����4')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainMenu',0,'����5','http://www.moyebuy.com',6,'_blank','NavItem','NavItem5','����5')
					
					
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('CategoryAll',0,'ȫ����Ʒ����','http://www.moyebuy.com',1,'_self','CategorysTitle','CategorysTitle','ȫ����Ʒ����')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('Category',0,'ʳƷ�����ϡ���ˮ','http://www.moyebuy.com',2,'_self',null,'Categorys1','ʳƷ�����ϡ���ˮ')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('Category',0,'������Ʒ�����˻���','http://www.moyebuy.com',3,'_self',null,'Categorys2','������Ʒ�����˻���')
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('Category',0,'��Ʒ���3','http://www.moyebuy.com',4,'_self',null,'Categorys3','��Ʒ���')

				
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',0,'��Ʒ','http://www.moyebuy.com',0,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','��Ʒ')
				
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',0,'����','http://www.moyebuy.com',1,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','����')
		
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',1,'�˵�����','http://www.moyebuy.com',0,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','�˵�����')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('MainSubCategory',1,'��Ʒ����','http://www.moyebuy.com',1,'_self','CategorysSubMenuTitle','CategorysSubMenuTitle','��Ʒ����')
				
		
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'��Ʒ1','http://www.moyebuy.com',1,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','��Ʒ1')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'��Ʒ2','http://www.moyebuy.com',2,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','��Ʒ2')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'��Ʒ��Ʒ��Ʒ3','http://www.moyebuy.com',3,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','��Ʒ3')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',0,'��Ʒ4','http://www.moyebuy.com',4,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','��Ʒ4')


INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'��Ӳ˵�','http://www.moyebuy.com',1,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','��Ӳ˵�')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'ɾ���˵�','http://www.moyebuy.com',2,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','ɾ���˵�')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'���²˵�','http://www.moyebuy.com',3,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','���²˵�')


INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'��Ӳ�Ʒ','http://www.moyebuy.com',1,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','��Ӳ�Ʒ')

INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'ɾ����Ʒ','http://www.moyebuy.com',2,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','ɾ����Ʒ')
					
INSERT INTO tbl_LayoutMenu(MenuType,IsAdminMenu,MenuName,MenuURL,MenuDisq,MenuTarget,MenuClassName,MenuControlID,MenuTitle)
					VALUES('SubCategory',1,'���²�Ʒ','http://www.moyebuy.com',3,'_self','CategorysSubMenuDesc','CategorysSubMenuDesc','���²�Ʒ')






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



