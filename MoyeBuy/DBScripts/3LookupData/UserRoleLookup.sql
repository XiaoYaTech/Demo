USE [MoyeBuyComServices]
Go
Delete tbl_Role
GO
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('SUPADMIN','超级管理员',GETDATE())
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('ADMIN','管理员',GETDATE())
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('USER','普通用户',GETDATE())
INSERT INTO tbl_Role(RoleName,RoleDesc,LastUpdatedDate) values('GUEST','游客',GETDATE())
