USE [Expose178ComLog]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'Expose178ComLog')
BEGIN
	exec sp_addlogin 'Expose178ComLog' ,'Expose178.Com', 'Expose178ComLog'
END
IF not exists(select * from [Expose178ComLog].dbo.sysusers where name=N'Expose178ComLog')
BEGIN
exec sp_grantdbaccess 'Expose178ComLog'
END
exec sp_addrolemember 'db_owner', 'Expose178ComLog'



--------------------------------------------------------------
USE [Expose178Com]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'Expose178Com')
BEGIN
	exec sp_addlogin 'Expose178Com' ,'Expose178.Com', 'Expose178Com'
END

IF not exists(select * from [Expose178Com].dbo.sysusers where name=N'Expose178Com')
BEGIN
exec sp_grantdbaccess 'Expose178Com'
END

IF not exists(select * from [Expose178Com].dbo.sysusers where name=N'Expose178ComLog')
BEGIN
exec sp_grantdbaccess 'Expose178ComLog'
END

exec sp_addrolemember 'db_owner', 'Expose178Com'
exec sp_addrolemember 'db_owner', 'Expose178ComLog'



