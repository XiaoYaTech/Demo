USE [MoyeBuyComServices]
Go
Delete tbl_Role
GO
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('SUPADMIN','��������Ա',GETDATE())
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('ADMIN','����Ա',GETDATE())
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('USER','��ͨ�û�',GETDATE())
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('GUEST','�ο�',GETDATE())
