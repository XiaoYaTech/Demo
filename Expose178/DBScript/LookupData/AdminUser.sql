USE [Expose178Com]
GO

exec usp_AddUpdateUserInfoByUserID
@UserName='Admin',
@UserEmail='Admin@expose178.com',
@PwdSalt='aa',
@PwdHash='bb',
@IsEffective=1,
@IsNeedChangePwd=0
