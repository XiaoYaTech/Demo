USE [GiftCardService]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'GiftCardService')
BEGIN
	exec sp_addlogin 'GiftCardService' ,'MoyeBuy.Com', 'GiftCardService'
END
IF not exists(select * from [GiftCardService].dbo.sysusers where name=N'GiftCardService')
BEGIN
exec sp_grantdbaccess 'GiftCardService'
END
exec sp_addrolemember 'db_owner', 'GiftCardService'


------------------------------------------------------
USE [MoyeBuyCom]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'MoyeBuyCom')
BEGIN
	exec sp_addlogin 'MoyeBuyCom' ,'MoyeBuy.Com', 'MoyeBuyCom'
END
IF not exists(select * from [MoyeBuyCom].dbo.sysusers where name=N'MoyeBuyCom')
BEGIN
exec sp_grantdbaccess 'MoyeBuyCom'
END
exec sp_addrolemember 'db_owner', 'MoyeBuyCom'


----------------------------------------------------------
USE [MoyeBuyComLog]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'MoyeBuyComLog')
BEGIN
	exec sp_addlogin 'MoyeBuyComLog' ,'MoyeBuy.Com', 'MoyeBuyComLog'
END
IF not exists(select * from [MoyeBuyComLog].dbo.sysusers where name=N'MoyeBuyComLog')
BEGIN
exec sp_grantdbaccess 'MoyeBuyComLog'
END
exec sp_addrolemember 'db_owner', 'MoyeBuyComLog'


----------------------------------------------------------
USE [MoyeBuyComOrder]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'MoyeBuyComOrder')
BEGIN
	exec sp_addlogin 'MoyeBuyComOrder' ,'MoyeBuy.Com', 'MoyeBuyComOrder'
END
IF not exists(select * from [MoyeBuyComOrder].dbo.sysusers where name=N'MoyeBuyComOrder')
BEGIN
exec sp_grantdbaccess 'MoyeBuyComOrder'
END
exec sp_addrolemember 'db_owner', 'MoyeBuyComOrder'


----------------------------------------------------------
USE [MoyeBuyComServices]
GO

IF not exists (select * from master.dbo.syslogins where loginname = N'MoyeBuyComServices')
BEGIN
	exec sp_addlogin 'MoyeBuyComServices' ,'MoyeBuy.Com', 'MoyeBuyComServices'
END
IF not exists(select * from [MoyeBuyComServices].dbo.sysusers where name=N'MoyeBuyComServices')
BEGIN
exec sp_grantdbaccess 'MoyeBuyComServices'
END

IF not exists(select * from [MoyeBuyComServices].dbo.sysusers where name=N'GiftCardService')
BEGIN
exec sp_grantdbaccess 'GiftCardService'
END

IF not exists(select * from [MoyeBuyComServices].dbo.sysusers where name=N'MoyeBuyCom')
BEGIN
exec sp_grantdbaccess 'MoyeBuyCom'
END

IF not exists(select * from [MoyeBuyComServices].dbo.sysusers where name=N'MoyeBuyComLog')
BEGIN
exec sp_grantdbaccess 'MoyeBuyComLog'
END

IF not exists(select * from [MoyeBuyComServices].dbo.sysusers where name=N'MoyeBuyComOrder')
BEGIN
exec sp_grantdbaccess 'MoyeBuyComOrder'
END

exec sp_addrolemember 'db_owner', 'MoyeBuyComServices'
exec sp_addrolemember 'db_owner', 'GiftCardService'
exec sp_addrolemember 'db_owner', 'MoyeBuyCom'
exec sp_addrolemember 'db_owner', 'MoyeBuyComLog'
exec sp_addrolemember 'db_owner', 'MoyeBuyComOrder'




